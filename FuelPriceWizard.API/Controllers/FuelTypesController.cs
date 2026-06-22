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
    public class FuelTypesController(ILogger<FuelTypesController> logger, IMapper mapper, IFuelPriceWizardService service) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PagedResult<FuelTypeDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var (items, total) = await service.GetFuelTypesPagedAsync(page, pageSize, ct);
            return Ok(new PagedResult<FuelTypeDto>
            {
                Items = mapper.Map<IEnumerable<FuelTypeDto>>(items),
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
            });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<FuelTypeDto>> GetById(int id, CancellationToken ct = default)
        {
            var fuelType = await service.GetFuelTypeByIdAsync(id, ct);
            if (fuelType is null)
            {
                logger.LogWarning("No fuel type found with id {Id}!", id);
                return NotFound();
            }
            return Ok(mapper.Map<FuelTypeDto>(fuelType));
        }

        [Authorize]
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<FuelTypeDto>> Insert([FromBody] FuelTypeDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inserted = await service.CreateFuelTypeAsync(mapper.Map<FuelType>(dto), ct);
            return Created(Url.Action(nameof(GetById), new { id = inserted.Id }), mapper.Map<FuelTypeDto>(inserted));
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<FuelTypeDto>> Update(int id, [FromBody] FuelTypeDto dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await service.UpdateFuelTypeAsync(id, mapper.Map<FuelType>(dto), ct);
                return Ok(mapper.Map<FuelTypeDto>(updated));
            }
            catch (NotFoundException) { return NotFound(); }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var deleted = await service.DeleteFuelTypeAsync(id, ct);
            return deleted ? NoContent() : NotFound();
        }
    }
}
