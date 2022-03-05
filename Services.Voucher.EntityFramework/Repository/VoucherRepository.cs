using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Services.Voucher.Application.Repository;
using Services.Voucher.Domain.Models;
using Services.Voucher.EntityFramework.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Voucher.EntityFramework.Repository
{
    /// <summary>
    /// An implementation of the <see cref="IVoucherRepository"/> interface using EntityFramework Core as the underlying database technology.
    /// </summary>
    public class VoucherRepository : IVoucherRepository
    {
        private readonly VoucherContext _voucherContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes an instance of the <see cref="VoucherRepository"/> object.
        /// </summary>
        /// <param name="voucherContext">A DbContext instance to access the database.</param>
        /// /// <param name="mapper">An IMapper implementation. Provided by AutoMapper.</param>
        public VoucherRepository(VoucherContext voucherContext, IMapper mapper)
        {
            _voucherContext = voucherContext;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<VoucherModel>> GetVouchers()
        {
            return _mapper.Map<IEnumerable<VoucherModel>>(_voucherContext.Vouchers);
        }

        /// <inheritdoc/>
        public async Task<VoucherModel> GetVoucherById(Guid id)
        {
            var voucher = await _voucherContext.Vouchers.FindAsync(id);
            return voucher == null ? null : _mapper.Map<VoucherModel>(voucher);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<VoucherModel>> GetVouchersByName(string name)
        {
            var vouchers = await _voucherContext.Vouchers.Where(v => v.Name == name).ToArrayAsync();
            return _mapper.Map<IEnumerable<VoucherModel>>(vouchers);
        }
    }
}
