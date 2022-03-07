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
        /// <param name="take">The number of records to take.</param>
        /// <param name="skip">The number of records to skip.</param>
        /// <returns>A collection of voucher entities.</returns>
        Task<IEnumerable<VoucherModel>> GetVouchers(int take, int skip);

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

        /// <summary>
        /// Searches for vouchers whose name contains the search text.
        /// </summary>
        /// <param name="searchText">The text to search for in the name field.</param>
        /// <returns>A collection of voucher entities.</returns>
        Task<IEnumerable<VoucherModel>> SearchVouchersByName(string searchText);

        /// <summary>
        /// Looks up vouchers by product code and returns the cheapest one.
        /// </summary>
        /// <param name="productCode">The product code to search for.</param>
        /// <returns>A voucher entity.</returns>
        Task<VoucherModel> GetCheapestVoucherByProductCode(string productCode);
    }
}
