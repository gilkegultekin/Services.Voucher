using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Services.Voucher.Application.Repository;
using Services.Voucher.EntityFramework.Contexts;
using Services.Voucher.EntityFramework.Repository;
using System;
using System.Collections.Generic;
using System.IO;

namespace Services.Voucher
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddDbContextPool<VoucherContext>(builder =>
            {
                builder.UseInMemoryDatabase(databaseName: "VoucherDB");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, VoucherContext voucherContext)
        {
            SeedData(voucherContext);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void SeedData(VoucherContext voucherContext)
        {
            var text = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}data.json");
            var vouchers = JsonConvert.DeserializeObject<IEnumerable<EntityFramework.Models.Voucher>>(text);
            voucherContext.AddRange(vouchers);
            voucherContext.SaveChanges();
        }
    }
}
