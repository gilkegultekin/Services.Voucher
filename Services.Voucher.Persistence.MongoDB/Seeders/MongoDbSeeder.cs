using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Services.Voucher.Application.Seeders;
using Services.Voucher.Persistence.MongoDB.Contexts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Services.Voucher.Persistence.MongoDB.Seeders
{
    public class MongoDbSeeder : IDatabaseSeeder
    {
        private readonly IVoucherMongoDbContext _voucherMongoDbContext;
        private readonly ILogger<MongoDbSeeder> _logger;

        public MongoDbSeeder(IVoucherMongoDbContext voucherMongoDbContext, ILogger<MongoDbSeeder> logger)
        {
            _voucherMongoDbContext = voucherMongoDbContext;
            _logger = logger;
        }

        public void Seed()
        {
            var entities = _voucherMongoDbContext.Vouchers
                .Find(FilterDefinition<Models.Voucher>.Empty)
                .Limit(10)
                .ToList();

            if (entities == null || !entities.Any())
            {
                _logger.LogInformation("Could not find any entities in the database. Beginning seeding process...");
                var text = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}data.json");
                var voucherModels = JsonConvert.DeserializeObject<IEnumerable<Models.Voucher>>(text);
                _voucherMongoDbContext.Vouchers.InsertMany(voucherModels);

                _logger.LogInformation("Seeding completed successfully.");
            }
            else
            {
                _logger.LogInformation("Database has already been seeded.");
            }
        }
    }
}
