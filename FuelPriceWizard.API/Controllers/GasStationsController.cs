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
    public class GasStationsController(ILogger<GasStationsController> logger, IMapper mapper, IFuelPriceWizardService service) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PagedResult<GasStationDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var (items, total) = await service.GetGasStationsPagedAsync(page, pageSize, ct);
            return Ok(new PagedResult<GasStationDto>
            {
                Items = mapper.Map<IEnumerable<GasStationDto>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
            });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<GasStationDto>> GetById(int id, CancellationToken ct = default)
        {
            var gasStation = await service.GetGasStationByIdAsync(id, ct);
            if (gasStation is null)
            {
                logger.LogWarning("No gas station found with id {Id}!", id);
                return NotFound();
            }
            return Ok(mapper.Map<GasStationDto>(gasStation));
        }

        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<GasStationDto>> Insert([FromBody] GasStationDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid gas station provided: {GasStation}!", dto);
                return BadRequest(ModelState);
            }
            var inserted = await service.CreateGasStationAsync(mapper.Map<GasStation>(dto), ct);
            var resourceUri = Url.Action(nameof(GetById), new { id = inserted.Id });
            return Created(resourceUri, mapper.Map<GasStationDto>(inserted));
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<GasStationDto>> Update(int id, [FromBody] GasStationDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid gas station provided: {GasStation}!", dto);
                return BadRequest(ModelState);
            }
            try
            {
                var updated = await service.UpdateGasStationAsync(id, mapper.Map<GasStation>(dto), ct);
                return Ok(mapper.Map<GasStationDto>(updated));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var deleted = await service.DeleteGasStationAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
