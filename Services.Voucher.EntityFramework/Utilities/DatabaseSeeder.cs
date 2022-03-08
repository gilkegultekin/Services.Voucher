using Newtonsoft.Json;
using Services.Voucher.Application.Dto;
using Services.Voucher.EntityFramework.Contexts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Services.Voucher.EntityFramework.Utilities
{
    public class DatabaseSeeder
    {
        public static void Seed(VoucherContext voucherContext)
        {
            var text = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}data.json");
            var voucherModels = JsonConvert.DeserializeObject<IEnumerable<VoucherDto>>(text);
            voucherContext.Vouchers.AddRange(voucherModels.Select(v => new Models.Voucher
            {
                Id = v.Id,
                Name = v.Name,
                Price = v.Price,
            }));

            voucherContext.ProductCodes.AddRange(voucherModels
                .SelectMany(v => v.ProductCodes.Split(','))
                .Distinct()
                .Select(s => new Models.ProductCode
                {
                    Id = s,
                }));

            foreach (var voucherModel in voucherModels)
            {
                foreach (var productCode in voucherModel.ProductCodes.Split(',').Distinct())
                {
                    voucherContext.VoucherProductCodes.Add(new Models.VoucherProductCode
                    {
                        Id = Guid.NewGuid(),
                        VoucherId = voucherModel.Id,
                        ProductCodeId = productCode,
                    });
                }

            }
            voucherContext.SaveChanges();
        }
    }
}
