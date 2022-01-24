using Entities.Identity;

namespace System_Analysis.Models
{
    public class Channel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ChannelId { get; set; }
        public ICollection<User> Users { get; set; }
        public Guid AdminId { get; set; }
        public ICollection<GroupMessage> GroupMessages { get; set; }
        public ChannelType ChannelType { get; set; }
        public string ChannelPicture { get; set; }
        public string About { get; set; }
        public string Link { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public enum ChannelType
    {
        Private,
        Public
    }
}
