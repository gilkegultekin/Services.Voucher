using Services.Voucher.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Voucher.Application.Repository
{
    public interface IVoucherRepository
    {
        Task<IEnumerable<VoucherModel>> GetVouchers();
    }
}
