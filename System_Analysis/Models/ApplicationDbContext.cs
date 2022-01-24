using Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System_Analysis.Models.Bank;

namespace System_Analysis.Models
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
        public DbSet<BankCard> BankCards { get; set; }
        public DbSet<CardToCard> CardToCards { get; set; }
        public DbSet<HousingFacility> HousingFacilities { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Bot> Bots { get; set; }
        public DbSet<Channel> Channels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var entitiesAssembly = typeof(User).Assembly;

            builder.RegisterEntityTypeConfiguration(entitiesAssembly);
        }
        public override int SaveChanges()
        {
            _updateTracker();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _updateTracker();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _updateTracker();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _updateTracker();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void _updateTracker()
        {
            var changedEntriesCopy = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
            e.State == EntityState.Modified ||
            e.State == EntityState.Deleted)
            .ToList();
            var saveTime = DateTime.Now;

            foreach (var entityEntry in changedEntriesCopy)
            {
                if (entityEntry.Metadata.FindProperty("UpdatedAt") != null)
                {
                    entityEntry.Property("UpdatedAt").CurrentValue = saveTime;
                }
            }
        }
    }
    public static class ModelBuilderExtension
    {
        /// <summary>
        /// Dynamicaly load all IEntityTypeConfiguration with Reflection
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assemblies">Assemblies contains Entities</param>
        public static void RegisterEntityTypeConfiguration(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            MethodInfo applyGenericMethod = typeof(ModelBuilder)
                .GetMethods()
                .First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration));

            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic);

            foreach (Type type in types)
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface.IsConstructedGenericType && iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        MethodInfo applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(modelBuilder, new object[] { Activator.CreateInstance(type) });
                    }
                }
            }
        }
    }
}
