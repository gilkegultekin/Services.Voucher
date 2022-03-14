using MongoDB.Driver;

namespace Services.Voucher.Persistence.MongoDB.Contexts
{
    public interface IVoucherMongoDbContext
    {
        IMongoCollection<Models.Voucher> Vouchers { get; }
    }
}