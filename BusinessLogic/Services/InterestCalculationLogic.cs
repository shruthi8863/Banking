using BankingApplication.BusinessLogic.Interfaces;
using BankingApplication.BusinessLogic.Models;
using Newtonsoft.Json;
using NLog;
using BankingApplication.Helpers;

namespace BankingApplication.BusinessLogic.Services
{
    internal class InterestCalculationLogic : IInterestCalculationLogic
    {

        private static ILogger _logger = LogManager.GetCurrentClassLogger();
        public async Task UpdateInterestRates(DateTime currentDate)
        {

            DateTime lastDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            if (currentDate.Date == lastDayOfMonth.Date)
            {
                #region get all the transactions from the AccountsInterest

                var accountsJson = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "AccountInterests.json"));
                var allTransactionsOfTheMonth = JsonConvert.DeserializeObject<List<AccountInterest>>(accountsJson);

                if (allTransactionsOfTheMonth is not null)
                {

                    #region get all the rules from the rates
                    var ratesJson = await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Rates.json"));
                    var availableRule = JsonConvert.DeserializeObject<List<RatesRuleDetails>>(ratesJson);

                    #endregion

                    allTransactionsOfTheMonth = allTransactionsOfTheMonth.Where(x => DateTime.ParseExact(x.TransactionDate, "yyyyMMdd", null).ToString("MM") == currentDate.ToString("MM")).ToList();
                    var groupEdAccountNumber = allTransactionsOfTheMonth.GroupBy(x => x.AccountNumber).Select(x => x).ToList();


                    foreach (var account in groupEdAccountNumber)
                    {

                        var transactionPerDay = allTransactionsOfTheMonth.Where(x => x.AccountNumber == account.Key.ToString()).ToList();


                        foreach (var accountDetails in transactionPerDay)
                        {
                            decimal? cumulativeInterest = 0;
                            var currentIndex = transactionPerDay.IndexOf(accountDetails);
                            if (currentIndex != transactionPerDay.Count() - 1)
                            {
                                var nextIndex = transactionPerDay.IndexOf(accountDetails) + 1;
                                AccountInterest accountInterest = transactionPerDay[nextIndex];
                                var noOfDays = (DateTime.ParseExact(accountInterest.TransactionDate, "yyyyMMdd", null) - DateTime.ParseExact(accountDetails.TransactionDate, "yyyyMMdd", null)).ToString("dd");

                                var dates = Enumerable.Range(0, int.MaxValue)
                                                      .Select(index => DateTime.ParseExact(accountDetails.TransactionDate, "yyyyMMdd", null).AddDays(index))
                                                      .TakeWhile(date => date < DateTime.ParseExact(accountInterest.TransactionDate, "yyyyMMdd", null))
                                                      .ToList();

                                #region to check if there are any new rules added between the selected dates
                                var toCheckIfNewRatesArePresent = availableRule?.Where(x => DateTime.ParseExact(x.Date, "yyyyMMdd", null) > DateTime.ParseExact(accountDetails.TransactionDate, "yyyyMMdd", null) &&
                                DateTime.ParseExact(x.Date, "yyyyMMdd", null) > DateTime.ParseExact(accountInterest.TransactionDate, "yyyyMMdd", null)).Count();


                                if (toCheckIfNewRatesArePresent == 0)
                                {
                                    var interestRate = availableRule?.OrderByDescending(x => DateTime.ParseExact(x.Date, "yyyyMMdd", null)).FirstOrDefault()?.Rate;
                                    cumulativeInterest = (accountDetails.EodBalance * Convert.ToDecimal(noOfDays) * interestRate) / 100;
                                }
                                else
                                {
                                    foreach (var date in dates)
                                    {
                                        var interestRate = availableRule?.OrderByDescending(x => DateTime.ParseExact(x.Date, "yyyyMMdd", null)).FirstOrDefault()?.Rate;
                                        cumulativeInterest = (accountDetails.EodBalance * interestRate) / 100;
                                    }

                                }
                                #endregion

                            }

                            else if (accountDetails.TransactionDate.IsLastDayOftheMonth(lastDayOfMonth))
                            {
                                var noOfDays = (lastDayOfMonth - DateTime.ParseExact(accountDetails.TransactionDate, "yyyyMMdd", null)).ToString("dd");

                                var dates = Enumerable.Range(0, int.MaxValue)
                                                      .Select(index => DateTime.ParseExact(accountDetails.TransactionDate, "yyyyMMdd", null).AddDays(index))
                                                      .TakeWhile(date => date <= lastDayOfMonth)
                                                      .ToList();
                                var toCheckIfNewRatesArePresent = availableRule?.Where(x => DateTime.ParseExact(x.Date, "yyyyMMdd", null) > DateTime.ParseExact(accountDetails.TransactionDate, "yyyyMMdd", null) &&
                                DateTime.ParseExact(x.Date, "yyyyMMdd", null) > lastDayOfMonth).Count();


                                if (toCheckIfNewRatesArePresent == 0)
                                {
                                    var interestRate = availableRule?.OrderByDescending(x => DateTime.ParseExact(x.Date, "yyyyMMdd", null)).FirstOrDefault()?.Rate;
                                    cumulativeInterest = (accountDetails.EodBalance * Convert.ToDecimal(noOfDays) * interestRate) / 100;
                                }
                                else
                                {
                                    foreach (var date in dates)
                                    {
                                        var interestRate = availableRule?.OrderByDescending(x => DateTime.ParseExact(x.Date, "yyyyMMdd", null)).FirstOrDefault()?.Rate;
                                        cumulativeInterest = (accountDetails.EodBalance * interestRate) / 100;
                                    }

                                }


                            }



                            var interestForTheMonth = Math.Round(cumulativeInterest.Value / 365);

                        }


                    }

                }


                else
                {
                    _logger.Log(LogLevel.Info, string.Format("There is no account created for this month :{0}", currentDate.ToString("MMMM")));
                }
                #endregion
            }
            else
            {
                _logger.Log(LogLevel.Info, string.Format("Interest will be calculated at the end of month"));
            }

        }




    }

}
