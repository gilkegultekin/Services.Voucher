using Microsoft.EntityFrameworkCore;

namespace Services.Voucher.EntityFramework.Contexts
{
    public class VoucherContext : DbContext
    {
        
        public VoucherContext(DbContextOptions<VoucherContext> options) : base(options)
        {
        }

        public DbSet<Models.Voucher> Vouchers { get; set; }

        public DbSet<Models.ProductCode> ProductCodes { get; set; }

        public DbSet<Models.VoucherProductCode> VoucherProductCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.VoucherProductCode>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Models.VoucherProductCode>()
                .HasOne(x => x.Voucher)
                .WithMany(v => v.VoucherProductCodes)
                .HasForeignKey(x => x.VoucherId);

            modelBuilder.Entity<Models.VoucherProductCode>()
                .HasOne(x => x.ProductCode)
                .WithMany(pc => pc.VoucherProductCodes)
                .HasForeignKey(x => x.ProductCodeId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
