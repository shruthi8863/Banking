using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BankingApplication.BusinessLogic.Models
{
    public class TransactionDetails
    {
        public string Account { get; set; } =string.Empty;

        public List<InputTransactions> InputTransactions { get; set; } = new();

    }

    public class InputTransactions
    {


        public string Date { get; set; }

        [JsonProperty("Txn Id")]
        public string TransactionId { get; set; }

        public string Type { get; set; }

        public decimal Amount { get; set; }

        public decimal CurrentBalance { get; set; } = 0;
    }
}