using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Voucher.Application.Dto;
using Services.Voucher.Application.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        /// <param name="take">Indicates page size. The default value is 1000. Max value is 8192.</param>
        /// <param name="skip">The number of voucher objects to skip. The default value is 0.</param>
        /// <returns>A collection of voucher dto objects.</returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<VoucherDto>>> Get([Range(0, int.MaxValue)]int take = 0, [Range(0, int.MaxValue)] int skip = 0)
        {
            var vouchers = await _voucherRepository.GetVouchers(take, skip);
            var dtoCollection = _mapper.Map<IEnumerable<VoucherDto>>(vouchers);
            return Ok(dtoCollection);
        }

        /// <summary>
        /// Looks up a voucher object by its id.
        /// </summary>
        /// <param name="id">The id of requested the voucher object.</param>
        /// <returns>A single voucher object.</returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<VoucherDto>> GetVoucherById(Guid id)
        {
            var voucher = await _voucherRepository.GetVoucherById(id);
            if (voucher == null)
            {
                return NotFound();
            }
            var dto = _mapper.Map<VoucherDto>(voucher);
            return Ok(dto);
        }

        /// <summary>
        /// Looks up voucher objects by their name.
        /// </summary>
        /// <param name="name">The name of the requested voucher objects.</param>
        /// <returns>A collection of voucher objects.</returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<VoucherDto>>> GetVouchersByName(string name)
        {
            var vouchers = await _voucherRepository.GetVouchersByName(name);
            return Ok(_mapper.Map<IEnumerable<VoucherDto>>(vouchers));
        }

        /// <summary>
        /// Searches for vouchers whose name contains the search text.
        /// </summary>
        /// <param name="search">The text to search for.</param>
        /// <returns>A collection of voucher objects.</returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<IEnumerable<VoucherDto>>> GetVouchersByNameSearch(string search)
        {
            var vouchers = await _voucherRepository.SearchVouchersByName(search);
            return Ok(_mapper.Map<IEnumerable<VoucherDto>>(vouchers));
        }

        /// <summary>
        /// Looks up vouchers by product code and returns the cheapest one among them.
        /// </summary>
        /// <param name="productCode">The product code to search for.</param>
        /// <returns>A single voucher object.</returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<VoucherDto>> GetCheapestVoucherByProductCode(string productCode)
        {
            var cheapest = await _voucherRepository.GetCheapestVoucherByProductCode(productCode);
            var dto = _mapper.Map<VoucherDto>(cheapest);
            return Ok(dto);
        }
    }
}
