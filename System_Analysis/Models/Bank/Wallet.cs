using Entities.Identity;

namespace System_Analysis.Models.Bank
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public decimal Value { get; set; }
    }
}
