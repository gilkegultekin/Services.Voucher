using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Services.Voucher.Application.Repository;
using Services.Voucher.Application.Seeders;
using Services.Voucher.Core.Extensions;
using Services.Voucher.EntityFramework.Contexts;
using Services.Voucher.EntityFramework.Repository;
using Services.Voucher.EntityFramework.Utilities;
using Services.Voucher.MapperProfiles;
using Services.Voucher.Persistence.MongoDB.Contexts;
using Services.Voucher.Persistence.MongoDB.Extensions;
using Services.Voucher.Persistence.MongoDB.MapperProfiles;
using Services.Voucher.Persistence.MongoDB.Settings;
using System;
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

            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDBSettings"));
            services.AddMongoDbServices();

            //services.AddScoped<IVoucherRepository, VoucherRepository>();
            //services.AddDbContextPool<VoucherContext>(builder =>
            //{
            //    builder.UseInMemoryDatabase(databaseName: "VoucherDB");
            //    builder.EnableSensitiveDataLogging();
            //});

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //services.AddAutoMapper(typeof(VoucherMongoDbProfile), typeof(VoucherDtoProfile));
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

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                };

            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "voucher_api");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDatabaseSeeder databaseSeeder)
        {
            //DatabaseSeeder.Seed(voucherContext);
            databaseSeeder.Seed();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization("ApiScope");
            });
        }
    }
}
