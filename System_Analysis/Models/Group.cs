using Entities.Identity;

namespace System_Analysis.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Guid AdminId { get; set; }
        public ICollection<GroupMessage> GroupMessages { get; set; }
    }
}
