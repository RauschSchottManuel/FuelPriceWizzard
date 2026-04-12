using AutoMapper;
using FuelPriceWizard.API.DTOs;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FuelPriceWizard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ILogger<CurrenciesController> logger;
        private readonly IMapper mapper;
        private readonly ICurrencyRepository currencyRepository;

        public CurrenciesController(ILogger<CurrenciesController> logger, IMapper mapper, ICurrencyRepository currencyRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.currencyRepository = currencyRepository;
        }

        /// <summary>Returns all currencies.</summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<CurrencyDto>), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetAll()
        {
            var currencies = await currencyRepository.GetAllAsync();
            return Ok(mapper.Map<IEnumerable<CurrencyDto>>(currencies));
        }

        /// <summary>Returns a currency by its identifier.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<CurrencyDto>> GetById(int id)
        {
            var currency = await currencyRepository.GetByIdAsync(id);
            if (currency is null)
            {
                logger.LogWarning("No currency found with id {Id}!", id);
                return NotFound();
            }

            return Ok(mapper.Map<CurrencyDto>(currency));
        }

        /// <summary>Creates a new currency.</summary>
        [HttpPost("new")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<CurrencyDto>> InsertNew([FromBody] CurrencyDto currency)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid currency provided: {Currency}!", currency);
                return BadRequest(ModelState);
            }

            var inserted = await currencyRepository.InsertAsync(mapper.Map<Currency>(currency));
            var resourceUri = Url.Action(nameof(GetById), new { id = inserted.Id });
            return Created(resourceUri, mapper.Map<CurrencyDto>(inserted));
        }

        /// <summary>Updates an existing currency.</summary>
        [HttpPut("edit/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<CurrencyDto>> Update(int id, [FromBody] CurrencyDto currency)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid currency provided: {Currency}!", currency);
                return BadRequest(ModelState);
            }

            var updated = await currencyRepository.UpdateAsync(id, mapper.Map<Currency>(currency));
            return Ok(mapper.Map<CurrencyDto>(updated));
        }

        /// <summary>Deletes a currency by its identifier.</summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            await currencyRepository.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
