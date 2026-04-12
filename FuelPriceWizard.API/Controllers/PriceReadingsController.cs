using AutoMapper;
using FuelPriceWizard.API.DTOs;
using FuelPriceWizard.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FuelPriceWizard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceReadingsController : ControllerBase
    {
        private readonly ILogger<PriceReadingsController> logger;
        private readonly IMapper mapper;
        private readonly IFuelPriceWizardService fuelPriceWizardService;

        public PriceReadingsController(ILogger<PriceReadingsController> logger, IMapper mapper, IFuelPriceWizardService fuelPriceWizardService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.fuelPriceWizardService = fuelPriceWizardService;
        }

        /// <summary>Returns the most recent price reading per fuel type for the given gas station.</summary>
        [HttpGet("{stationId}/latest")]
        [ProducesResponseType(typeof(IEnumerable<PriceReadingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<PriceReadingDto>>> GetLatest(int stationId)
        {
            var station = await fuelPriceWizardService.GetGasStationByIdAsync(stationId);
            if (station is null)
            {
                logger.LogWarning("No gas station found with id {Id}!", stationId);
                return NotFound();
            }

            var prices = await fuelPriceWizardService.GetLatestPricesForStationAsync(stationId);
            return Ok(mapper.Map<IEnumerable<PriceReadingDto>>(prices));
        }

        /// <summary>Returns price history for a specific fuel type at the given gas station within a date range.</summary>
        /// <param name="stationId">Gas station identifier.</param>
        /// <param name="fuelTypeId">Fuel type identifier.</param>
        /// <param name="from">Start of the date range (UTC). Defaults to 7 days ago.</param>
        /// <param name="to">End of the date range (UTC). Defaults to now.</param>
        [HttpGet("{stationId}/history")]
        [ProducesResponseType(typeof(IEnumerable<PriceReadingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<PriceReadingDto>>> GetHistory(
            int stationId,
            [FromQuery] int fuelTypeId,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var station = await fuelPriceWizardService.GetGasStationByIdAsync(stationId);
            if (station is null)
            {
                logger.LogWarning("No gas station found with id {Id}!", stationId);
                return NotFound();
            }

            var rangeFrom = from ?? DateTime.UtcNow.AddDays(-7);
            var rangeTo = to ?? DateTime.UtcNow;

            if (rangeFrom > rangeTo)
                return BadRequest("'from' must be earlier than 'to'.");

            var prices = await fuelPriceWizardService.GetPriceHistoryAsync(stationId, fuelTypeId, rangeFrom, rangeTo);
            return Ok(mapper.Map<IEnumerable<PriceReadingDto>>(prices));
        }
    }
}
