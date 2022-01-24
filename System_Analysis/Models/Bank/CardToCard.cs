namespace System_Analysis.Models.Bank
{
    public class CardToCard
    {
        public Guid Id { get; set; }
        public string DesCard { get; set; }
        public string SrcCard { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsSuccess { get; set; }
        public BankCard BankCard { get; set; }
        public Guid BankCardId { get; set; }
    }
}
