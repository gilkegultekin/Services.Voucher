using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Voucher.Application.Dto;
using Services.Voucher.Application.Repository;
using Services.Voucher.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Voucher.Controllers
{
    /// <summary>
    /// An API controller that operates on Voucher objects.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes an instance of the <see cref="VoucherController"/> object.
        /// </summary>
        /// <param name="voucherRepository">A repository implementation that manages voucher objects.</param>
        /// <param name="mapper">An IMapper implementation. Provided by AutoMapper.</param>
        public VoucherController(IVoucherRepository voucherRepository, IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets voucher objects in a paged manner.
        /// </summary>
        /// <param name="take">Indicates page size. The default value is 1000.</param>
        /// <param name="skip">The number of voucher objects to skip. The default value is 0.</param>
        /// <returns>A collection of voucher dto objects.</returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<VoucherDto>>> Get(int take = 1000, int skip = 0)
        {
            var vouchers = await _voucherRepository.GetVouchers();
            return Ok(_mapper.Map<IEnumerable<VoucherDto>>(vouchers.Skip(skip).Take(take)));
        }

        /// <summary>
        /// Looks up a voucher object by its id.
        /// </summary>
        /// <param name="id">The id of requested the voucher object.</param>
        /// <returns>A single voucher object.</returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<VoucherDto>> GetVoucherById(Guid id)
        {
            var voucher = await _voucherRepository.GetVoucherById(id);
            if (voucher == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VoucherDto>(voucher));
        }

        /// <summary>
        /// Looks up voucher objects by their name.
        /// </summary>
        /// <param name="name">The name of the requested voucher objects.</param>
        /// <returns>A collection of voucher objects.</returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<VoucherDto>>> GetVouchersByName(string name)
        {
            var vouchers = await _voucherRepository.GetVouchersByName(name);
            return Ok(_mapper.Map<IEnumerable<VoucherDto>>(vouchers));
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
