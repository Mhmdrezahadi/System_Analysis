namespace System_Analysis.Models.Bank
{
    public class HousingFacility
    {
        public Guid Id { get; set; }
        public string TrackingCode { get; set; }
        public DateTime DateTime { get; set; }
        public string FacilityCard { get; set; }
        public bool Status { get; set; }
        public BankCard BankCard { get; set; }
        public Guid BankCardId { get; set; }

    }
}
