using Microsoft.AspNetCore.Mvc;
using Services.Voucher.Application.Repository;
using Services.Voucher.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Voucher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherController(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<VoucherModel>> Get(int count = 0)
        {
            var vouchers = await _voucherRepository.GetVouchers();
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
        public async Task<VoucherModel> GetVoucherById(Guid id)
        {
            var vouchers = await _voucherRepository.GetVouchers();
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
        public async Task<IEnumerable<VoucherModel>> GetVouchersByName(string name)
        {
            var vouchers = await _voucherRepository.GetVouchers();
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
    }
}
