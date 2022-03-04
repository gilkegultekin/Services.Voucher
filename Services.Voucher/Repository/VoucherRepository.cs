using Newtonsoft.Json;
using Services.Voucher.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Services.Voucher.Repository
{
    public class VoucherRepository
    {
        internal string DataFilename = $"{AppDomain.CurrentDomain.BaseDirectory}data.json";

        private IEnumerable<VoucherModel> _vouchers;

        public IEnumerable<VoucherModel> GetVouchers()
        {
            if (_vouchers == null)
            {
                var text = File.ReadAllText(DataFilename);
                _vouchers = JsonConvert.DeserializeObject<IEnumerable<VoucherModel>>(text);
            }
            return _vouchers;
        }
    }
}
