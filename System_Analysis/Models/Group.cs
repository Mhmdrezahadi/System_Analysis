using Entities.Identity;

namespace System_Analysis.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
        public Guid AdminId { get; set; }
        public ICollection<GroupMessage> GroupMessages { get; set; }
    }
}
