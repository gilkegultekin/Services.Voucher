using Newtonsoft.Json;
using Services.Voucher.Application.Repository;
using Services.Voucher.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services.Voucher.FileDB.Repository
{
    public class VoucherRepository  : IVoucherRepository
    {
        internal string DataFilename = $"{AppDomain.CurrentDomain.BaseDirectory}data.json";

        private IEnumerable<VoucherModel> _vouchers;

        public Task<VoucherModel> GetCheapestVoucherByProductCode(string productCode)
        {
            throw new NotImplementedException();
        }

        public Task<VoucherModel> GetVoucherById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VoucherModel>> GetVouchers(int take, int skip)
        {
            if (_vouchers == null)
            {
                var text = File.ReadAllText(DataFilename);
                _vouchers = JsonConvert.DeserializeObject<IEnumerable<VoucherModel>>(text);
            }
            return Task.FromResult(_vouchers);
        }

        public Task<IEnumerable<VoucherModel>> GetVouchersByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VoucherModel>> SearchVouchersByName(string searchText, int take, int skip)
        {
            throw new NotImplementedException();
        }
    }
}
