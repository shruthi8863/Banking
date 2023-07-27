using BankingApplication.BusinessLogic.Interfaces;
using BankingApplication.BusinessLogic.Models;
using BankingApplication.Helpers;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;

namespace BankingApplication.BusinessLogic.Services
{
    public class ProcessTransactions : IProcessTransactions
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();


        #region Public Methods
        public async Task<ErrorResponses> UpsertTransactions(TransactionRequest transactions, string path)
        {

            ErrorResponses errorResponses = new();
            try
            {

                int transactionCount = 1;
                if (File.Exists(path))
                {
                    #region upsert transactions


                    var json = await File.ReadAllTextAsync(path);
                    var getTransactions = JsonConvert.DeserializeObject<List<TransactionDetails>>(json);

                    #region Find the account with the specified account Id
                    var exisitingAccount = getTransactions?.FirstOrDefault(x => x.Account == transactions.AccountNumber);
                    #endregion

                    if (exisitingAccount is not null)
                    {
                        string TrxnId = "1";
                        InputTransactions inputTransactions = new();
                        inputTransactions.Date = transactions.Date;
                        inputTransactions.TransactionId = generateUniqueId(transactions.Date, exisitingAccount, ref TrxnId);
                        inputTransactions.Amount = Convert.ToDecimal(transactions.Amount);
                        inputTransactions.Type = transactions.transactionTypes;

                        #region currentBalance calculation
                        decimal currentBalance = 0;
                        if (transactions.transactionTypes == "D")
                        {
                            currentBalance = exisitingAccount.InputTransactions.Sum(x => x.Amount) + inputTransactions.Amount;
                            inputTransactions.CurrentBalance = currentBalance;
                        }
                        else
                        {
                            currentBalance = exisitingAccount.InputTransactions.Sum(x => x.Amount) - inputTransactions.Amount;
                            if (currentBalance == 0)
                            {
                                errorResponses.Errors.Add("Account balance cannot made as zero, so current transaction is not possible");
                            }
                            else
                            {
                                inputTransactions.CurrentBalance = currentBalance;
                            }
                        }
                        #endregion
                        #endregion

                        exisitingAccount.InputTransactions.Add(inputTransactions);


                        List<TransactionDetails> transactionDetails = new List<TransactionDetails>();
                        transactionDetails.Add(exisitingAccount);
                        getTransactions = transactionDetails;
                        #region to calculate the EOD balance 


                        
                        UpdateOrCreate(getTransactions, path);

                        #region to make entry for interest calculation


                        AccountInterest accountInterest = new()
                        {
                            AccountNumber = transactions.AccountNumber,
                            TransactionDate = transactions.Date,
                            InterestCalculated = false,
                            EodBalance = currentBalance
                        };


                        accountInterest.AccountDetailsForTransactions(accountInterest, Path.Combine(Directory.GetCurrentDirectory(), "AccountInterests.json"));
                        #endregion


                    }
                    else
                    {
                        if (transactions.transactionTypes != "W")
                        {

                            TransactionDetails transactionDetail = new();
                            transactionDetail.Account = transactions.AccountNumber;
                            InputTransactions inputTransactions = new();
                            inputTransactions.Date = transactions.Date;
                            inputTransactions.TransactionId = string.Format(transactions.Date + "-" + transactionCount);
                            inputTransactions.Amount = Convert.ToDecimal(transactions.Amount);
                            inputTransactions.Type = transactions.transactionTypes;
                            transactionDetail.InputTransactions.Add(inputTransactions);
                            getTransactions = new();
                            getTransactions?.Add(transactionDetail);
                            UpdateOrCreate(getTransactions, path);


                            AccountInterest accountInterest = new()
                            {
                                AccountNumber = transactions.AccountNumber,
                                TransactionDate = transactions.Date,
                                InterestCalculated = false,
                                EodBalance = inputTransactions.Amount
                            };
                            accountInterest.AccountDetailsForTransactions(accountInterest, Path.Combine(Directory.GetCurrentDirectory(), "AccountInterests.json"));


                        }

                        else
                        {
                            Console.WriteLine(string.Format("The first transaction of the account cannot be a withdrawal:{0}", transactions.AccountNumber));
                            
                        }
                    }

                }


                #endregion


                else
                {
                    if (transactions.transactionTypes != "W")
                    {
                        TransactionDetails transactionDetail = new();
                        transactionDetail.Account = transactions.AccountNumber;
                        InputTransactions inputTransactions = new();
                        inputTransactions.Date = transactions.Date;
                        inputTransactions.TransactionId = string.Format(transactions.Date + "-" + transactionCount);
                        inputTransactions.Amount = Convert.ToDecimal(transactions.Amount);
                        inputTransactions.Type = transactions.transactionTypes;
                        transactionDetail.InputTransactions.Add(inputTransactions);

                        List<TransactionDetails> transactionDetails = new();
                        transactionDetails.Add(transactionDetail);


                        UpdateOrCreate(transactionDetails, path);
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return errorResponses;
        }

        public async Task<ErrorResponses> UpsertRates(RatesRuleRequest ratesRuleRequest, string path)
        {
            ErrorResponses errorResponses = new();
            try
            {
                List<RatesRuleDetails> ratesRuleDetails = null;
                if (File.Exists(path))
                {
                    #region Find the rule with the specified Rule Id and date condition

                    var json = File.ReadAllText(path);
                    var getRates = JsonConvert.DeserializeObject<List<RatesRuleDetails>>(json);
                    var existingRates = getRates?.FirstOrDefault(x => x.Date == ratesRuleRequest.Date);
                    if (existingRates != null)
                    {
                        existingRates.Rate = Convert.ToDecimal(ratesRuleRequest.Rate);
                        ratesRuleDetails = new();
                        ratesRuleDetails.Add(existingRates);
                        getRates = ratesRuleDetails;
                        UpdateOrCreate(getRates, path);

                    }
                    else
                    {
                        RatesRuleDetails ratesRuleDetail = new();
                        ratesRuleDetail.RuleId = ratesRuleRequest.RuleId;
                        ratesRuleDetail.Rate = Convert.ToDecimal(ratesRuleRequest.Rate);
                        ratesRuleDetail.Date = ratesRuleRequest.Date;
                        getRates?.Add(ratesRuleDetail);
                        UpdateOrCreate(getRates, path);
                    }
                    #endregion
                }


            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, string.Format("An unexpected error has occured :{0}", ex));

            }
            return errorResponses;
        }

        public  async Task<TransactionDetails> PrintTransactions(string[] printInputs, string path)
        {
            TransactionDetails transactions = null;


            try
            {
                var json = await File.ReadAllTextAsync(path);
                var getTransactions = JsonConvert.DeserializeObject<List<TransactionDetails>>(json);

                TransactionDetails? transactionDetails = getTransactions?.FirstOrDefault(x => x.Account == printInputs[0]);
                if (transactionDetails is not null)
                {

                    var res = transactionDetails.InputTransactions.Where(x => DateTime.ParseExact(x.Date, "yyyyMMdd", null).ToString("MM") == DateTime.ParseExact(printInputs[1], "MM", null).ToString("MM"));
                    transactions = new();
                    transactions.InputTransactions = new();
                    transactions.InputTransactions= res.ToList();
                    transactions.Account = transactionDetails.Account;
                }
                else
                {
                    Console.WriteLine(String.Format("Specified account number does not exist :{0}", printInputs[0]));
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, string.Format("An unexpected error has occured :{0}", ex));
            }
            return transactions;
        }

        #endregion

        #region private Methods
        private void UpdateOrCreate<T>(List<T>? transactionDetails, string path)
        {
            string json = JsonConvert.SerializeObject(transactionDetails, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(path, json);
        }


        private string generateUniqueId(string transactionDate, TransactionDetails exisitingAccount, ref string TrxnId)
        {
            var existingTransactionCount = exisitingAccount.InputTransactions.Where(x => x.Date == transactionDate).Count();
            return TrxnId = transactionDate + "-" + (existingTransactionCount + 1);
        }
        #endregion

    }
}
