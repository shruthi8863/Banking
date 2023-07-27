namespace BankingApplication.BusinessLogic.Models
{
    public class TransactionRequest
    {
        public int Id { get; set; }
 
        public string AccountNumber { get; set; } = string.Empty;

        public string transactionTypes { get; set; }= string.Empty;

        public string Amount { get; set; } = string.Empty;

        public string Date { get; set; } = string.Empty;
    }

    public enum TransactionTypes
    {
        D,W
    }
}

