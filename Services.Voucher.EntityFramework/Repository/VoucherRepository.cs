using Services.Voucher.Application.Repository;
using Services.Voucher.Domain.Models;
using Services.Voucher.EntityFramework.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Services.Voucher.EntityFramework.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly VoucherContext voucherContext;

        public VoucherRepository(VoucherContext voucherContext)
        {
            this.voucherContext = voucherContext;
        }

        public async Task<IEnumerable<VoucherModel>> GetVouchers()
        {
            return voucherContext.Vouchers
                .Select(x => new VoucherModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ProductCodes = x.ProductCodes,
                }).AsEnumerable();
        }
    }
}
