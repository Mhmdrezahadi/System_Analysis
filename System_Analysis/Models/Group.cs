using Entities.Identity;

namespace System_Analysis.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public User Admin { get; set; }
        public ICollection<GroupMessage> GroupMessages { get; set; }
    }
}
