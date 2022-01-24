namespace System_Analysis.Models.Bank
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string TrackingCode { get; set; }
        public Wallet Wallet { get; set; }
        public Guid WalletId { get; set; }
    }
}
