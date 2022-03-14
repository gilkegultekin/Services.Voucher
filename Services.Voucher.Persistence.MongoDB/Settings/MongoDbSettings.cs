namespace Services.Voucher.Persistence.MongoDB.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string VoucherCollectionName { get; set; }
    }
}
