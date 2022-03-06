using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using Services.Voucher.Application.Repository;
using Services.Voucher.Core.Extensions;
using Services.Voucher.EntityFramework.Contexts;
using Services.Voucher.EntityFramework.MapperProfiles;
using Services.Voucher.EntityFramework.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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

            //Required by Serilog's CorrelationId enricher.
            services.AddHttpContextAccessor();

            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddDbContextPool<VoucherContext>(builder =>
            {
                builder.UseInMemoryDatabase(databaseName: "VoucherDB");
            });
            //TODO: Find a better way to register these profiles. Probably have to use reflection to find all subclasses of Profile.
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(VoucherEntityProfile).Assembly);
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Domino's Voucher API",
                    Description = "An ASP.NET Core Web API for managing vouchers",
                    TermsOfService = new Uri("https://www.dominos.nl/over-dominos/algemene-voorwaarden"),
                    Contact = new OpenApiContact
                    {
                        Name = "Contact",
                        Url = new Uri("https://www.dominos.nl/over-dominos/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("https://example.com/license")
                    }
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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

            app.UseGlobalExceptionHandler();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

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
