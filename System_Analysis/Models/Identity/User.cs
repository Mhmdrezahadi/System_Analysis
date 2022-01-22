using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System_Analysis.Models;

namespace Entities.Identity
{
    /// <summary>
    /// کاربر
    /// </summary>
    public class User : IdentityUser<Guid>
    {
        /// <summary>
        /// نام
        /// </summary>
        [PersonalData]
        public string FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [PersonalData]
        public string LastName { get; set; }

        /// <summary>
        /// تصویر
        /// </summary>
        [PersonalData]
        public string SnapShot { get; set; }

        [PersonalData]
        public string Province { get; set; }

        [PersonalData]
        public string City { get; set; }

        public bool Status { get; set; } = true;

        public virtual ICollection<UserClaim> Claims { get; set; }

        public virtual ICollection<UserLogin> Logins { get; set; }

        public virtual ICollection<UserToken> Tokens { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Group> Rooms { get; set; }
        public ICollection<GroupMessage> GroupMessages { get; set; }
        public ICollection<PrivateMessage> PrivateMessages { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UserConfiguartion : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            // Each User can have many UserClaims
            builder.HasMany(e => e.Claims)
            .WithOne(e => e.User)
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();

            // Each User can have many UserLogins
            builder.HasMany(e => e.Logins)
            .WithOne(e => e.User)
            .HasForeignKey(ul => ul.UserId)
            .IsRequired();

            // Each User can have many UserTokens
            builder.HasMany(e => e.Tokens)
            .WithOne(e => e.User)
            .HasForeignKey(ut => ut.UserId)
            .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
            .WithOne(e => e.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }

}
