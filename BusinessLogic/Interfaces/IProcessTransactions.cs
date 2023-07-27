using BankingApplication.BusinessLogic.Models;
using BankingApplication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.BusinessLogic.Interfaces
{
    public interface IProcessTransactions
    {
        Task<ErrorResponses> UpsertTransactions(TransactionRequest trasanctionRequest, string path);
        
        Task<ErrorResponses> UpsertRates(RatesRuleRequest ratesRuleRequest, string path);
        
        Task<TransactionDetails> PrintTransactions(string[] printInputs, string path);
    }
}
