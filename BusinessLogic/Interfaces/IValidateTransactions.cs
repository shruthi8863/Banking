using BankingApplication.BusinessLogic.Models;
using BankingApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.BusinessLogic.Interfaces
{
    public interface IValidateTransactions
    {
        ErrorResponses ValidateTransactionsAsync(TransactionRequest trasanctionRequest);
        ErrorResponses ValidateRatesAsync(RatesRuleRequest ratesRuleRequest);
    }
}
