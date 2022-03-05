using Newtonsoft.Json;
using Services.Voucher.Application.Repository;
using Services.Voucher.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services.Voucher.FileDB
{
    public class VoucherRepository : IVoucherRepository
    {
        internal string DataFilename = $"{AppDomain.CurrentDomain.BaseDirectory}data.json";

        private IEnumerable<VoucherModel> _vouchers;

        public Task<IEnumerable<VoucherModel>> GetVouchers()
        {
            if (_vouchers == null)
            {
                var text = File.ReadAllText(DataFilename);
                _vouchers = JsonConvert.DeserializeObject<IEnumerable<VoucherModel>>(text);
            }
            return Task.FromResult(_vouchers);
        }
    }
}
