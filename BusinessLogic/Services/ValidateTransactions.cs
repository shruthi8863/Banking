using BankingApplication.BusinessLogic.Interfaces;
using BankingApplication.BusinessLogic.Models;
using BankingApplication.Helpers;
using System.Text.RegularExpressions;
using NLog;

namespace BankingApplication.BusinessLogic.Services
{
    public class ValidateTransactions : IValidateTransactions 
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        public ErrorResponses ValidateTransactionsAsync(TransactionRequest transaction)
        {
            ErrorResponses errorResponses = new();
            try
            {
                if (transaction != null)
                {
                    Regex regEx = new Regex(@"^(19|20)\d\d(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])$");
                    if (!regEx.IsMatch(transaction.Date))
                    {
                        errorResponses.Errors.Add(transaction.Date + "is not a valid Date format for the system");
                    }
                    TransactionTypes result;
                    if (!Enum.TryParse<TransactionTypes>(transaction.transactionTypes, true, out result))
                    {
                        errorResponses.Errors.Add(transaction.transactionTypes + "is not a valid transaction type for the system");
                    }


                    if (transaction.Amount == "0.00"&& transaction.Amount.Split('.')[1]?.Length!=2)

                    {
                        errorResponses.Errors.Add(transaction.Amount + "is not a valid Amount  for the system");
                    }

                    if(transaction.Date.IsFutureDate() || transaction.Date.IsPastDate())
                    {
                        errorResponses.Errors.Add(transaction.Date + "cannot be a future date or a past date");
                    }
                    errorResponses.ErrorCount = errorResponses.Errors.Count();
                }
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, string.Format("An unexpected error has occured :{0}", ex));
            }
            return errorResponses;
        }


   

        public ErrorResponses ValidateRatesAsync(RatesRuleRequest ratesRule)
        {
            ErrorResponses errorResponses = new();
            try
            {
                if (ratesRule != null)
                {
                    Regex regEx = new Regex(@"^(19|20)\d\d(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])$");
                    if (!regEx.IsMatch(ratesRule.Date))
                    {
                        errorResponses.Errors.Add(ratesRule.Date + "is not a valid Date format accepted by the system");
                    }


                    if ((ratesRule.Rate == "0.00" || ratesRule.Rate == "100.00") && ratesRule.Rate.Split('.')[1]?.Length != 2)

                    {
                        errorResponses.Errors.Add(ratesRule.Rate + "is not a valid rate  accepted by the system");
                    }

                    errorResponses.ErrorCount = errorResponses.Errors.Count();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, string.Format("An unexpected error has occured :{0}", ex));

            }
            return errorResponses;
        }
    }
}
