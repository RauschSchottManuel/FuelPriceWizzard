using AutoMapper;
using FuelPriceWizard.API.DTOs;
using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FuelPriceWizard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceReadingsController(ILogger<PriceReadingsController> logger, IMapper mapper, IFuelPriceWizardService service) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PagedResult<PriceReadingDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var (items, total) = await service.GetPriceReadingsPagedAsync(page, pageSize, ct);
            return Ok(new PagedResult<PriceReadingDto>
            {
                Items = mapper.Map<IEnumerable<PriceReadingDto>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
            });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PriceReadingDto>> GetById(int id, CancellationToken ct = default)
        {
            var reading = await service.GetPriceReadingByIdAsync(id, ct);
            if (reading is null)
            {
                logger.LogWarning("No price reading found with id {Id}!", id);
                return NotFound();
            }
            return Ok(mapper.Map<PriceReadingDto>(reading));
        }

        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PriceReadingDto>> Insert([FromBody] PriceReadingDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inserted = await service.CreatePriceReadingAsync(mapper.Map<PriceReading>(dto), ct);
            return Created(Url.Action(nameof(GetById), new { id = inserted.Id }), mapper.Map<PriceReadingDto>(inserted));
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var deleted = await service.DeletePriceReadingAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
