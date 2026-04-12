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
    public class FuelTypesController : ControllerBase
    {
        private readonly ILogger<FuelTypesController> logger;
        private readonly IMapper mapper;
        private readonly IFuelTypeRepository fuelTypeRepository;

        public FuelTypesController(ILogger<FuelTypesController> logger, IMapper mapper, IFuelTypeRepository fuelTypeRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.fuelTypeRepository = fuelTypeRepository;
        }

        /// <summary>Returns all fuel types.</summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<FuelTypeDto>), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<FuelTypeDto>>> GetAll()
        {
            var fuelTypes = await fuelTypeRepository.GetAllAsync();
            return Ok(mapper.Map<IEnumerable<FuelTypeDto>>(fuelTypes));
        }

        /// <summary>Returns a fuel type by its identifier.</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FuelTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<FuelTypeDto>> GetById(int id)
        {
            var fuelType = await fuelTypeRepository.GetByIdAsync(id);
            if (fuelType is null)
            {
                logger.LogWarning("No fuel type found with id {Id}!", id);
                return NotFound();
            }

            return Ok(mapper.Map<FuelTypeDto>(fuelType));
        }

        /// <summary>Creates a new fuel type.</summary>
        [HttpPost("new")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FuelTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<FuelTypeDto>> InsertNew([FromBody] FuelTypeDto fuelType)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid fuel type provided: {FuelType}!", fuelType);
                return BadRequest(ModelState);
            }

            var inserted = await fuelTypeRepository.InsertAsync(mapper.Map<FuelType>(fuelType));
            var resourceUri = Url.Action(nameof(GetById), new { id = inserted.Id });
            return Created(resourceUri, mapper.Map<FuelTypeDto>(inserted));
        }

        /// <summary>Updates an existing fuel type.</summary>
        [HttpPut("edit/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FuelTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<FuelTypeDto>> Update(int id, [FromBody] FuelTypeDto fuelType)
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Invalid fuel type provided: {FuelType}!", fuelType);
                return BadRequest(ModelState);
            }

            var updated = await fuelTypeRepository.UpdateAsync(id, mapper.Map<FuelType>(fuelType));
            return Ok(mapper.Map<FuelTypeDto>(updated));
        }

        /// <summary>Deletes a fuel type by its identifier.</summary>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id)
        {
            await fuelTypeRepository.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
