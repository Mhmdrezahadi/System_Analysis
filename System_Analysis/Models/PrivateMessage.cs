using Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace System_Analysis.Models
{
    public class PrivateMessage
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Guid ToUserId { get; set; }
    }
    public class PrivateMessageFromUserConfiguartion : IEntityTypeConfiguration<PrivateMessage>
    {
        public void Configure(EntityTypeBuilder<PrivateMessage> builder)
        {
        }
    }
}
