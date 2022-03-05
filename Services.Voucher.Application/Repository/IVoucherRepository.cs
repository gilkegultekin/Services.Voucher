using Services.Voucher.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Voucher.Application.Repository
{
    /// <summary>
    /// The repository interface for managing vouchers.
    /// </summary>
    public interface IVoucherRepository
    {
        /// <summary>
        /// Gets all vouchers.
        /// </summary>
        /// <returns>A collection of voucher entities.</returns>
        Task<IEnumerable<VoucherModel>> GetVouchers();

        /// <summary>
        /// Looks up a single voucher by its id.
        /// </summary>
        /// <param name="id">The id of the requested voucher object.</param>
        /// <returns>A voucher entity.</returns>
        Task<VoucherModel> GetVoucherById(Guid id);

        /// <summary>
        /// Looks up vouchers by their name.
        /// </summary>
        /// <param name="name">The name of the requested vouchers.</param>
        /// <returns>A collection of voucher entities.</returns>
        Task<IEnumerable<VoucherModel>> GetVouchersByName(string name);
    }
}
