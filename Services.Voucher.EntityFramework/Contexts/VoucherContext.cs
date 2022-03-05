using Microsoft.EntityFrameworkCore;

namespace Services.Voucher.EntityFramework.Contexts
{
    public class VoucherContext : DbContext
    {
        public VoucherContext(DbContextOptions<VoucherContext> options) : base(options)
        {

        }

        public DbSet<Models.Voucher> Vouchers { get; set; }
    }
}
