using AutoMapper;
using FuelPriceWizard.API.DTOs;
using FuelPriceWizard.API.Models;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FuelPriceWizard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GasStationsController : ControllerBase
    {
        private readonly ILogger<GasStationsController> logger;
        private readonly IMapper mapper;
        private readonly IGasStationRepository gasStationRepository;

        public GasStationsController(ILogger<GasStationsController> logger, IMapper mapper, IGasStationRepository gasStationRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.gasStationRepository = gasStationRepository;
        }

        /// <summary>Returns a paginated list of all gas stations.</summary>
        /// <param name="page">Page number (1-based). Defaults to 1.</param>
        /// <param name="pageSize">Number of items per page. Defaults to 20.</param>
        [HttpGet("all")]
        [ProducesResponseType(typeof(PagedResult<GasStationDto>), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PagedResult<GasStationDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            var allStations = (await gasStationRepository.GetAllAsync()).ToList();
            var items = allStations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new PagedResult<GasStationDto>
            {
                Items = mapper.Map<IEnumerable<GasStationDto>>(items),
                TotalCount = allStations.Count,
                Page = page,
                PageSize = pageSize,
            });
        }

        /// <summary>Returns a gas station by its identifier.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GasStationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<GasStationDto>> GetById(int id)
        {
            var gasStation = await gasStationRepository.GetByIdAsync(id);

            if (gasStation is null)
            {
                logger.LogWarning("No gas station found with id {Id}!", id);
                return NotFound();
            }

            return Ok(mapper.Map<GasStationDto>(gasStation));
        }

        /// <summary>Creates a new gas station.</summary>
        [HttpPost("new")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(GasStationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<GasStationDto>> InsertNew([FromBody] GasStationDto gasStation)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid gas station provided: {GasStation}!", gasStation);
                return BadRequest(ModelState);
            }

            var inserted = await gasStationRepository.InsertAsync(mapper.Map<GasStation>(gasStation));
            var resourceUri = Url.Action(nameof(GetById), new { id = inserted.Id });
            return Created(resourceUri, mapper.Map<GasStationDto>(inserted));
        }

        /// <summary>Updates an existing gas station.</summary>
        [HttpPut("edit/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(GasStationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<GasStationDto>> Update(int id, [FromBody] GasStationDto gasStation)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid gas station provided: {GasStation}!", gasStation);
                return BadRequest(ModelState);
            }

            var updated = await gasStationRepository.UpdateAsync(id, mapper.Map<GasStation>(gasStation));
            return Ok(mapper.Map<GasStationDto>(updated));
        }

        /// <summary>Deletes a gas station by its identifier.</summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            await gasStationRepository.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
