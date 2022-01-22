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
        public ICollection<User> Users { get; set; }
        //public User FromUser { get; set; }
        //public Guid FromUserId { get; set; }
        //public User ToUser { get; set; }
        //public Guid ToUserId { get; set; }
    }
    public class PrivateMessageConfiguartion : IEntityTypeConfiguration<PrivateMessage>
    {
        public void Configure(EntityTypeBuilder<PrivateMessage> builder)
        {
            //builder.HasOne(p => p.FromUser).WithMany(p => p.PrivateMessages).HasForeignKey(p => p.FromUserId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //builder.HasOne(p => p.ToUser).WithMany(p => p.PrivateMessages).HasForeignKey(p => p.ToUserId)
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
