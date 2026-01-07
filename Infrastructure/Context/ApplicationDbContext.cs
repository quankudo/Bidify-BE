using bidify_be.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace bidify_be.Infrastructure.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<PackageBid> PackageBid => Set<PackageBid>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<GiftType> GiftTypes => Set<GiftType>();
        public DbSet<Gift> Gifts => Set<Gift>();
        public DbSet<Voucher> Vouchers => Set<Voucher>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<ProductTag> ProductTags => Set<ProductTag>();
        public DbSet<FileStorage> FileStorages => Set<FileStorage>();
        public DbSet<TopupTransaction> TopupTransactions { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<TransitionPackageBid> TransitionPackagesBids { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionTag> AuctionTags { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            builder.Entity<ApplicationUser>()
               .HasIndex(u => u.ReferralCode)
               .IsUnique();
        }
    }
}
