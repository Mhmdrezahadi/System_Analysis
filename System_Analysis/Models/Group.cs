using Entities.Identity;

namespace System_Analysis.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string GroupId { get; set; }
        public ICollection<User> Users { get; set; }
        public Guid AdminId { get; set; }
        public ICollection<GroupMessage> GroupMessages { get; set; }
        public GroupType GroupType { get; set; }
        public string GroupPicture { get; set; }
        public string About { get; set; }
        public string Link { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
    public enum GroupType
    {
        Private,
        Public
    }
}
