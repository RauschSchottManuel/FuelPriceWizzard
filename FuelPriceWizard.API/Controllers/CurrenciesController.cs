using AutoMapper;
using FuelPriceWizard.API.DTOs;
using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.DataAccess.Exceptions;
using FuelPriceWizard.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FuelPriceWizard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController(ILogger<CurrenciesController> logger, IMapper mapper, IFuelPriceWizardService service) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PagedResult<CurrencyDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var (items, total) = await service.GetCurrenciesPagedAsync(page, pageSize, ct);
            return Ok(new PagedResult<CurrencyDto>
            {
                Items = mapper.Map<IEnumerable<CurrencyDto>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
            });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<CurrencyDto>> GetById(int id, CancellationToken ct = default)
        {
            var currency = await service.GetCurrencyByIdAsync(id, ct);
            if (currency is null)
            {
                logger.LogWarning("No currency found with id {Id}!", id);
                return NotFound();
            }
            return Ok(mapper.Map<CurrencyDto>(currency));
        }

        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<CurrencyDto>> Insert([FromBody] CurrencyDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inserted = await service.CreateCurrencyAsync(mapper.Map<Currency>(dto), ct);
            return Created(Url.Action(nameof(GetById), new { id = inserted.Id }), mapper.Map<CurrencyDto>(inserted));
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<CurrencyDto>> Update(int id, [FromBody] CurrencyDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await service.UpdateCurrencyAsync(id, mapper.Map<Currency>(dto), ct);
                return Ok(mapper.Map<CurrencyDto>(updated));
            }
            catch (NotFoundException) { return NotFound(); }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var deleted = await service.DeleteCurrencyAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
