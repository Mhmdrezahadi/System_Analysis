using Entities.Identity;

namespace System_Analysis.Models.Bank
{
    public class BankCard
    {
        public Guid Id { get; set; }
        public string CardNumber { get; set; }
        public DateTime dateTime { get; set; }
        public int CVV2 { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}
