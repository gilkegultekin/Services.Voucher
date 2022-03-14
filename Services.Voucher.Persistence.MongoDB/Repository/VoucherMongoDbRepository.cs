using MongoDB.Driver;
using Services.Voucher.Application.Repository;
using Services.Voucher.Core.Exceptions;
using Services.Voucher.Domain.Models;
using Services.Voucher.Persistence.MongoDB.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Voucher.Persistence.MongoDB.Repository
{
    public class VoucherMongoDbRepository : IVoucherRepository
    {
        private readonly IVoucherMongoDbContext _voucherMongoDbContext;
        //AutoMapper strips away custom mapping configuration and fails to map the product codes. https://github.com/AutoMapper/AutoMapper/issues/2043
        //private readonly IMapper _mapper;

        public VoucherMongoDbRepository(IVoucherMongoDbContext voucherMongoDbContext)
        {
            _voucherMongoDbContext = voucherMongoDbContext;
            //_mapper = mapper;
        }

        public async Task<VoucherModel> GetCheapestVoucherByProductCode(string productCode)
        {

            var cheapest = _voucherMongoDbContext.Vouchers.AsQueryable()
                .Where(v => v.ProductCodes.Contains(productCode))
                .OrderBy(v => v.Price)
                .FirstOrDefault();

            //var cursor = await _voucherMongoDbContext.Vouchers.FindAsync(v => v.ProductCodes.Contains(productCode));
            //var vouchers = await cursor.ToListAsync();
            //var cheapest = vouchers
            //    .OrderBy(v => v.Price)
            //    .FirstOrDefault();

            if (cheapest == null)
            {
                throw new CustomServiceException($"The specified product code {productCode} does not exist!");
            }

            return new VoucherModel(cheapest.Id, cheapest.Name, cheapest.Price, cheapest.ProductCodes.Split(','));
        }

        public async Task<VoucherModel> GetVoucherById(Guid id)
        {
            IAsyncCursor<Models.Voucher> cursor = await _voucherMongoDbContext.Vouchers.FindAsync(v => v.Id == id);
            var voucher = await cursor.FirstOrDefaultAsync();
            if (voucher == null)
            {
                return null;
            }

            return new VoucherModel(voucher.Id, voucher.Name, voucher.Price, voucher.ProductCodes.Split(','));
        }

        public async Task<IEnumerable<VoucherModel>> GetVouchers(int take, int skip)
        {
            if (skip == 0 && take == 0)
            {
                IAsyncCursor<Models.Voucher> cursor = await _voucherMongoDbContext.Vouchers.FindAsync(FilterDefinition<Models.Voucher>.Empty);
                var dbEntities = await cursor.ToListAsync();
                //return _mapper.Map<List<VoucherModel>>(dbEntities);
                return dbEntities.Select(x => new VoucherModel(x.Id, x.Name, x.Price, x.ProductCodes.Split(',')));
            }

            var pagedEntities = await _voucherMongoDbContext.Vouchers
                .Find(FilterDefinition<Models.Voucher>.Empty)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();
            //return _mapper.Map<List<VoucherModel>>(pagedEntities);
            return pagedEntities.Select(x => new VoucherModel(x.Id, x.Name, x.Price, x.ProductCodes.Split(',')));
        }

        public async Task<IEnumerable<VoucherModel>> GetVouchersByName(string name)
        {
            IAsyncCursor<Models.Voucher> cursor = await _voucherMongoDbContext.Vouchers.FindAsync(v => v.Name == name);
            var vouchers = await cursor.ToListAsync();
            if (vouchers == null || !vouchers.Any())
            {
                return Enumerable.Empty<VoucherModel>();
            }

            return vouchers.Select(x => new VoucherModel(x.Id, x.Name, x.Price, x.ProductCodes.Split(',')));
        }

        public async Task<IEnumerable<VoucherModel>> SearchVouchersByName(string searchText)
        {
            IAsyncCursor<Models.Voucher> cursor = await _voucherMongoDbContext.Vouchers.FindAsync(v => v.Name.Contains(searchText));
            var vouchers = await cursor.ToListAsync();
            if (vouchers == null || !vouchers.Any())
            {
                return Enumerable.Empty<VoucherModel>();
            }

            return vouchers.Select(x => new VoucherModel(x.Id, x.Name, x.Price, x.ProductCodes.Split(',')));
        }
    }
}
