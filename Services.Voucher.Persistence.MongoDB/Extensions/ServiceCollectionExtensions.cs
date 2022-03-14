using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Services.Voucher.Application.Repository;
using Services.Voucher.Application.Seeders;
using Services.Voucher.Persistence.MongoDB.Contexts;
using Services.Voucher.Persistence.MongoDB.Repository;
using Services.Voucher.Persistence.MongoDB.Seeders;
using Services.Voucher.Persistence.MongoDB.Settings;

namespace Services.Voucher.Persistence.MongoDB.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbServices(this IServiceCollection services)
        {
            services.AddScoped<IMongoClient>(sp =>
            {
                IOptions<MongoDbSettings> settings = sp.GetService<IOptions<MongoDbSettings>>();
                return new MongoClient(settings.Value.ConnectionString);
            });

            //IMongoDatabase registration
            services.AddScoped(sp =>
            {
                IMongoClient client = sp.GetRequiredService<IMongoClient>();
                IOptions<MongoDbSettings> settings = sp.GetService<IOptions<MongoDbSettings>>();
                return client.GetDatabase(settings.Value.DatabaseName);
            });

            services.AddScoped<IVoucherMongoDbContext, VoucherMongoDbContext>();
            services.AddScoped<IVoucherRepository, VoucherMongoDbRepository>();
            //Can't register as singleton as this class has dependencies on scoped services.
            services.AddTransient<IDatabaseSeeder, MongoDbSeeder>();

            return services;
        }
    }
}
