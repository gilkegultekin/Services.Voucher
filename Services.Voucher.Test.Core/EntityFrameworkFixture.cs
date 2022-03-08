using Bogus;
using Microsoft.EntityFrameworkCore;
using Services.Voucher.EntityFramework.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Voucher.Test.Core
{
    public class EntityFrameworkFixture : IDisposable
    {
        private readonly VoucherContext _dbContext;
        private readonly DbContextOptions<VoucherContext> _dbContextOptions;
        private List<VoucherContext> _contexts;

        public EntityFrameworkFixture()
        {
            _dbContextOptions = new DbContextOptionsBuilder<VoucherContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;
            _dbContext = new VoucherContext(_dbContextOptions);
            SeedDatabaseWithFakeEntities();
            _contexts = new List<VoucherContext> { _dbContext };
        }

        public VoucherContext GetNewContext()
        {
            var context = new VoucherContext(_dbContextOptions);
            _contexts.Add(context);
            return context;
        }

        public async Task InsertVouchersWithProductCode(string productCode, int min, int max, double minPrice = 5)
        {
            var faker = new Faker();
            //Choose a random product code among the existing ones to associate with the new vouchers, for the sake of product code diversity
            var randomProductCode = faker.PickRandom(await _dbContext.ProductCodes.Select(pc => pc.Id).ToArrayAsync());

            //Insert the new product code if necessary
            var productCodeInDb = await _dbContext.ProductCodes.FindAsync(productCode);
            if (productCodeInDb == null)
            {
                _dbContext.ProductCodes.Add(new EntityFramework.Models.ProductCode { Id = productCode });
            }

            //Generate new vouchers
            var voucherFaker = new Faker<EntityFramework.Models.Voucher>()
                .RuleFor(v => v.Id, f => f.Random.Guid())
                .RuleFor(v => v.Name, f => f.Random.Words())
                .RuleFor(v => v.Price, f => f.Random.Double(minPrice, 500));
            var vouchers = voucherFaker.GenerateBetween(min, max);
            _dbContext.Vouchers.AddRange(vouchers);
            
            //Associate the vouchers created in the prev stage with the given product code (and the randomly chosen one at the top)
            var voucherProductCodes = new List<EntityFramework.Models.VoucherProductCode>();
            foreach (var voucher in vouchers)
            {
                voucherProductCodes.Add(new EntityFramework.Models.VoucherProductCode { Id = faker.Random.Guid(), VoucherId = voucher.Id, ProductCodeId = productCode });
                voucherProductCodes.Add(new EntityFramework.Models.VoucherProductCode { Id = faker.Random.Guid(), VoucherId = voucher.Id, ProductCodeId = randomProductCode });
            }
            _dbContext.VoucherProductCodes.AddRange(voucherProductCodes);
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Guid> InsertVoucherWithProductCodeAndPrice(string productCode, double price)
        {
            var faker = new Faker();
            //Generate the id of the voucher to be inserted.
            var voucherId = Guid.NewGuid();
            //Insert the product code.
            _dbContext.ProductCodes.Add(new EntityFramework.Models.ProductCode { Id = productCode });
            //Generate and insert the voucher.
            _dbContext.Vouchers.Add(new EntityFramework.Models.Voucher { Id = voucherId, Name = faker.Random.Words(), Price = price });
            //Insert voucher-product code junction records.
            _dbContext.VoucherProductCodes.Add(new EntityFramework.Models.VoucherProductCode { Id = Guid.NewGuid(), VoucherId = voucherId, ProductCodeId = productCode });
            await _dbContext.SaveChangesAsync();
            return voucherId;
        }

        public async Task InsertVouchersWithName(string name, int count)
        {
            var faker = new Faker();
            //choose a small number of product codes at random to associate with the vouchers about to be created.
            var productCodes = faker.PickRandom(await _dbContext.ProductCodes.Select(pc => pc.Id).ToArrayAsync(), faker.Random.Int(1, 5));

            //Generate the vouchers with the specified name and insert them
            var voucherFaker = new Faker<EntityFramework.Models.Voucher>()
                .RuleFor(v => v.Id, f => f.Random.Guid())
                .RuleFor(v => v.Name, f => name)
                .RuleFor(v => v.Price, f => f.Random.Double(5, 500));
            var vouchers = voucherFaker.GenerateBetween(count, count);
            _dbContext.Vouchers.AddRange(vouchers);

            //Associate the vouchers with the product codes
            var voucherProductCodes = new List<EntityFramework.Models.VoucherProductCode>();
            foreach (var voucher in vouchers)
            {
                foreach (var productCode in productCodes)
                {
                    voucherProductCodes.Add(new EntityFramework.Models.VoucherProductCode { Id = faker.Random.Guid(), VoucherId = voucher.Id, ProductCodeId = productCode });
                }
            }
            _dbContext.VoucherProductCodes.AddRange(voucherProductCodes);

            await _dbContext.SaveChangesAsync();
        }

        public async Task InsertVoucherWithId(Guid id)
        {
            var faker = new Faker();
            //choose a product code at random to associate with the voucher about to be created.
            var productCode = await _dbContext.ProductCodes.FirstAsync();
            //insert the voucher with the specified id.
            _dbContext.Vouchers.Add(new EntityFramework.Models.Voucher { Id = id, Name = faker.Random.Words(), Price = faker.Random.Double(0, 100) });
            //insert the link between the voucher and the product code
            _dbContext.VoucherProductCodes.Add(new EntityFramework.Models.VoucherProductCode { Id = Guid.NewGuid(), VoucherId = id, ProductCodeId = productCode.Id });
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            foreach (var context in _contexts)
            {
                context.Dispose();
            }
        }

        private void SeedDatabaseWithFakeEntities()
        {
            var faker = new Faker();

            //Generate product codes and insert them.
            var productCodeFaker = new Faker<EntityFramework.Models.ProductCode>()
                .RuleFor(pc => pc.Id, f => f.Random.AlphaNumeric(5).ToUpperInvariant());
            var productCodes = productCodeFaker.GenerateBetween(40, 50);
            _dbContext.ProductCodes.AddRange(productCodes);

            //Generate vouchers and insert them.
            var voucherFaker = new Faker<EntityFramework.Models.Voucher>()
                .RuleFor(v => v.Id, f => f.Random.Guid())
                .RuleFor(v => v.Name, f => f.Random.Words())
                .RuleFor(v => v.Price, f => f.Random.Double(5, 500));
            var vouchers = voucherFaker.GenerateBetween(10000, 20000);
            _dbContext.Vouchers.AddRange(vouchers);

            //Associate the vouchers with existing product codes chosen at random
            var voucherProductCodes = new List<EntityFramework.Models.VoucherProductCode>();
            foreach (var voucher in vouchers)
            {
                var productCodeList = faker.PickRandom(productCodes, faker.Random.Int(1, 5));
                foreach (var productCode in productCodeList)
                {
                    voucherProductCodes.Add(new EntityFramework.Models.VoucherProductCode { Id = faker.Random.Guid(), VoucherId = voucher.Id, ProductCodeId = productCode.Id });
                }
            }
            _dbContext.VoucherProductCodes.AddRange(voucherProductCodes);

            _dbContext.SaveChanges();
        }
    }
}
