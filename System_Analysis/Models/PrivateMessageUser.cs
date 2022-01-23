using Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace System_Analysis.Models
{
    public class PrivateMessageUser
    {
        public PrivateMessage PrivateMessage { get; set; }
        public Guid PrivateMessageId { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
    public class PrivateMessageUserConfiguartion : IEntityTypeConfiguration<PrivateMessageUser>
    {
        public void Configure(EntityTypeBuilder<PrivateMessageUser> builder)
        {
            builder.HasKey(pm => new { pm.UserId, pm.PrivateMessageId });
        }
    }
}
