using BankingApplication.BusinessLogic.Interfaces;
using BankingApplication.BusinessLogic.Models;
using BankingApplication.Helpers;
using NLog;


namespace BankingApplication.BusinessLogic
{
    public class BankingTransactions
    {
        private readonly IValidateTransactions _validateTransactions;
        private readonly IProcessTransactions _processTransactions;
        private readonly ILogger _logger;
        public BankingTransactions(IValidateTransactions validateTransactions, IProcessTransactions processTransactions, ILogger logger)
        {
            _validateTransactions = validateTransactions;
            _processTransactions = processTransactions;
            _logger = logger;
        }
        public async Task ProcessInputs(string result)
        {
            try
            {
                
                switch (result.ToUpper())
                {

                    case "I":
                        Console.WriteLine("Please enter transaction details in <Date>|<Account>|<Type>|<Amount>");
                        var transactionInput = Console.ReadLine();
                        if (!string.IsNullOrEmpty(transactionInput) && transactionInput.Split("|").Length == 4)
                        {
                            string[] inputs = transactionInput.Split("|");
                            TransactionRequest trasanctionRequest = new();
                            trasanctionRequest.Date = inputs[0];
                            trasanctionRequest.AccountNumber = inputs[1];
                            trasanctionRequest.transactionTypes = inputs[2];
                            trasanctionRequest.Amount = inputs[3];

                            var res1 = _validateTransactions.ValidateTransactionsAsync(trasanctionRequest);

                            if (res1.ErrorCount == 0)
                            {

                                await _processTransactions.UpsertTransactions(trasanctionRequest, Path.Combine(Directory.GetCurrentDirectory(), "Transactions.json"));
                                //await GetAllAvailableTransactions();
                                Console.WriteLine(Constants.PROMPT_MORE);
                                Program.PromptForUserInput();

                            }
                            else
                            {
                                Console.WriteLine(res1.Errors.Aggregate("Please fix the errors", (s1, s2) => s1 + s2));
                            }
                        }
                        break;

                    case "D":

                        Console.WriteLine("Please enter interest rules details in <Date>|<RuleId>|<Rate in %> format");
                        var ratesInput = Console.ReadLine();
                        if (!string.IsNullOrEmpty(ratesInput) && ratesInput.Split("|").Length == 3)
                        {
                            RatesRuleRequest ratesRuleRequest = new();
                            string[] inputs = ratesInput.Split("|");
                            ratesRuleRequest.Date = inputs[0];
                            ratesRuleRequest.RuleId = inputs[1];
                            ratesRuleRequest.Rate = inputs[2];
                            var res = _validateTransactions.ValidateRatesAsync(ratesRuleRequest);
                            if (res.ErrorCount == 0)
                            {

                                await _processTransactions.UpsertRates(ratesRuleRequest, Path.Combine(Directory.GetCurrentDirectory(), "Resources","Rates.json"));
                                Console.WriteLine(Constants.PROMPT_MORE);
                                Program.PromptForUserInput();

                            }
                            else
                            {
                                Console.WriteLine(res.Errors.Aggregate("Please fix the errors", (s1, s2) => s1 + s2));
                            }
                        }

                        break;
                    case "P":

                        Console.WriteLine("Please enter account and month to generat the statement <Account>|<Month>");
                        var printInput = Console.ReadLine();
                        if(!string.IsNullOrEmpty(printInput) && printInput.Split("|").Length == 2)
                        {
                            string[] inputs = printInput.Split("|");
                            var outputToPrint= await _processTransactions.PrintTransactions(inputs, Path.Combine(Directory.GetCurrentDirectory(), "Transactions.json"));

                            Console.WriteLine($"Account : {outputToPrint.Account}");
                            Console.WriteLine(String.Join("|", "Date", "Txn Id", "Type", "Amount", "Balance"));

                            foreach (var transactions in outputToPrint.InputTransactions)
                            {
                                Console.WriteLine(String.Join("|", transactions.Date, transactions.TransactionId, transactions.Type, transactions.Amount, transactions.CurrentBalance));
                            }
                            Console.WriteLine(Constants.PROMPT_MORE);
                            Program.PromptForUserInput();

                        }
                        break;
                    case "Q":
                        Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
                        Console.WriteLine("Have a nice day!");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,string.Format("An unexpected error has occured :{0}", ex));
            }


        }


    }
}
