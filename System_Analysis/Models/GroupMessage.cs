using Entities.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace System_Analysis.Models
{
    public class GroupMessage
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public User FromUser { get; set; }
        public Group ToGroup { get; set; }
        [ForeignKey("ToGroup")]
        public Guid ToGroupId { get; set; }
    }
}
