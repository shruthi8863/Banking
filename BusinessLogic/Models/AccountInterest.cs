using BankingApplication.BusinessLogic.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BankingApplication.BusinessLogic.Models
{
    public class AccountInterest
    {
        [JsonProperty("accountNumber")]
      

        public string AccountNumber { get; set; }

        [JsonProperty("transactionDate")]
    
        public string TransactionDate { get; set; }

        [JsonProperty("isInterestCalculated")]
     
        public bool InterestCalculated { get; set; } =false;


        [JsonProperty("eodBalance")]

        public decimal EodBalance { get; set; }

        public void AccountDetailsForTransactions(AccountInterest accountInterest,string path)
        {

            if(File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var getTransactions = JsonConvert.DeserializeObject<List<AccountInterest>>(json);
                var exisitingAccount = getTransactions?.FirstOrDefault(x => x.AccountNumber == accountInterest.AccountNumber && x.TransactionDate == accountInterest.TransactionDate);

                if(exisitingAccount != null)
                {
                    exisitingAccount.TransactionDate  = accountInterest.TransactionDate;
                    exisitingAccount.EodBalance = accountInterest.EodBalance;
                    List<AccountInterest> accountInterests = new();
                    accountInterests.Add(exisitingAccount);
                    getTransactions = accountInterests;
                    UpdateOrCreateAccountDetailsForTransactions(getTransactions,path);

                }
                else
                {

                    #region  create new record for existing account
                    List<AccountInterest> accountInterests = new();
                    getTransactions.Add(accountInterest);
                    UpdateOrCreateAccountDetailsForTransactions(getTransactions, path);
                    #endregion

                }
            }
            else
            {

                #region  create  new record for non-existing account
                List<AccountInterest> accountInterests = new();
                accountInterests.Add(accountInterest);
                UpdateOrCreateAccountDetailsForTransactions(accountInterests, path);
                #endregion
            }

        }

        private void UpdateOrCreateAccountDetailsForTransactions(List<AccountInterest> accountInterests,string path)
        {
            string json = JsonConvert.SerializeObject(accountInterests , Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(path, json);
        }


    }
}
