using Entities.Identity;

namespace System_Analysis.Models
{
    public class Bot
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BotId { get; set; }
        public string About { get; set; }
        public Guid AdminId { get; set; }
        public ICollection<User> Users { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<GroupMessage> GroupMessages { get; set; }
    }
}
