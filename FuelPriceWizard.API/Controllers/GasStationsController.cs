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

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<GasStationDto>>> GetAll()
        {
            var gasStations = await this.gasStationRepository.GetAllAsync();
            return this.Ok(this.mapper.Map<IEnumerable<GasStationDto>>(gasStations));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<GasStationDto>> GetById(int id)
        {
            var gasStation = await this.gasStationRepository.GetByIdAsync(id);

            if(gasStation is null)
            {
                this.logger.LogWarning("No gas station found with id {Id}!", id);
                return this.NotFound();
            }

            return this.Ok(gasStation);
        }

        [HttpPost("new")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<GasStationDto>> InsertNew([FromBody] GasStationDto gasStation)
        {
            if(!ModelState.IsValid)
            {
                this.logger.LogError("Invalid gas station provided: {GasStation}!", gasStation);
                return this.BadRequest(ModelState);
            }

            var insertedStation = await this.gasStationRepository.InsertAsync(this.mapper.Map<GasStation>(gasStation));

            var resourceUri = Url.Action(nameof(GetById), new {id = insertedStation.Id});

            return this.Created(resourceUri, insertedStation);
        }

        [HttpPut("edit/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<GasStationDto>> Update(int id, [FromBody] GasStationDto gasStation)
        {
            if (!ModelState.IsValid)
            {
                this.logger.LogError("Invalid gas station provided: {GasStation}!", gasStation);
                return this.BadRequest(ModelState);
            }

            var updatedGasStation = await this.gasStationRepository.UpdateAsync(id, this.mapper.Map<GasStation>(gasStation));

            return this.Ok(updatedGasStation);
        }

        [HttpDelete("delete/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<GasStationDto>> Delete(int id)
        {
            var result = await this.gasStationRepository.DeleteByIdAsync(id);

            return this.NoContent();
        }

    }
}
