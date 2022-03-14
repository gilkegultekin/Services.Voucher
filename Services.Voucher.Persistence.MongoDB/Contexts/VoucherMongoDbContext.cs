using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Services.Voucher.Persistence.MongoDB.Settings;

namespace Services.Voucher.Persistence.MongoDB.Contexts
{
    public class VoucherMongoDbContext : IVoucherMongoDbContext
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly MongoDbSettings _mongoSettings;

        public VoucherMongoDbContext(IMongoDatabase mongoDatabase, IOptions<MongoDbSettings> mongoSettingsOptions)
        {
            _mongoDatabase = mongoDatabase;
            _mongoSettings = mongoSettingsOptions.Value;
        }

        public IMongoCollection<Models.Voucher> Vouchers => _mongoDatabase.GetCollection<Models.Voucher>(_mongoSettings.VoucherCollectionName);
    }
}
