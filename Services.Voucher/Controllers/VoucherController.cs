using Microsoft.AspNetCore.Mvc;
using Services.Voucher.Models;
using Services.Voucher.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Voucher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VoucherController : ControllerBase
    {
        private VoucherRepository _voucherRepository;

        [HttpGet]
        [Route("[action]")]
        public IEnumerable<VoucherModel> Get(int count = 0)
        {
            var vouchers = Repository.GetVouchers();
            if (count == 0)
            {
                count = vouchers.Count();
            }
            var returnVouchers = new List<VoucherModel>();
            for (var i = 0; i < count; i++)
            {
                returnVouchers.Add(vouchers.ElementAt(i));
            }
            return returnVouchers;
        }

        [HttpGet]
        [Route("[action]")]
        public VoucherModel GetVoucherById(Guid id)
        {
            var vouchers = Repository.GetVouchers();
            VoucherModel voucher = null;
            for (var i = 0; i < vouchers.Count(); i++)
            {
                if (vouchers.ElementAt(i).Id == id)
                {
                    voucher = vouchers.ElementAt(i);
                }
            }

            return voucher;
        }

        [HttpGet]
        [Route("[action]")]
        public IEnumerable<VoucherModel> GetVouchersByName(string name)
        {
            var vouchers = Repository.GetVouchers();
            var returnVouchers = new List<VoucherModel>();
            for (var i = 0; i < vouchers.Count(); i++)
            {
                if (vouchers.ElementAt(i).Name == name)
                {
                    returnVouchers.Add(vouchers.ElementAt(i));
                }
            }

            return returnVouchers.ToArray();
        }

        [HttpGet]
        [Route("[action]")]
        public IEnumerable<VoucherModel> GetVouchersByNameSearch(string search)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("[action]")]
        public VoucherModel GetCheapestVoucherByProductCode(string productCode)
        {
            throw new NotImplementedException();
        }

        public VoucherRepository Repository
        {
            get
            {
                return _voucherRepository ?? (_voucherRepository = new VoucherRepository());
            }
            set
            {
                _voucherRepository = value;
            }
        }
    }
}
