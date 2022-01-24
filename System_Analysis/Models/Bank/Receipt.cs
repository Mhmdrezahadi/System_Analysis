namespace System_Analysis.Models.Bank
{
    public class Receipt
    {
        public Guid Id { get; set; }
        public BankCard BankCard { get; set; }
        public Guid BankCardId { get; set; }
        public string TrackingCode { get; set; }
        public string BillingId { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
