namespace System_Analysis.Models.Bank
{
    public class Payment
    {
        public Guid Id { get; set; }
        public bool IsSuccess { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Operator { get; set; }
        public string MobileNumber { get; set; }
        public PaymentType PaymentType { get; set; }
        public string SimType { get; set; }
        public DateTime Time { get; set; }
        public string Volume { get; set; }
        public BankCard BankCard { get; set; }
        public Guid BankCardId { get; set; }
        public string TrackingCode { get; set; }
    }
    public enum PaymentType
    {
        PhoneCharge,
        Internet,
        // ,...
    }
}
