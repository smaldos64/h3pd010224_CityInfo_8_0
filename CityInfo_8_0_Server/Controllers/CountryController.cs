using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Services;
using ServicesContracts;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo_8_0_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;
        
#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif
        public CountryController(ILoggerManager logger,
                              IRepositoryWrapper repositoryWrapper)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
        }

        // GET: api/<CountryController>
        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries(string UserName = "No Name")
        {
            try
            {
                IEnumerable<Country> CountryList = new List<Country>();

                _repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();
                CountryList = await _repositoryWrapper.CountryRepositoryWrapper.FindAll();

                List<CountryDto> CountryDtos;

                CountryDtos = CountryList.Adapt<CountryDto[]>().ToList();
                //CountryDtos = UtilityService.MapCountryList(CountryList, true);

                _logger.LogInfo($"All Countries has been read from GetCountries action by {UserName}");
                return Ok(CountryDtos);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCities action for {UserName} : {Error.Message}");
                return StatusCode(500, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetCountry/{CountryId}")]
        public async Task<IActionResult> GetCountry(int CountryId,
                                                    string UserName = "No Name")
        {
            _repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();

            Country Country_Object = await _repositoryWrapper.CountryRepositoryWrapper.FindOne(CountryId);

            if (null == Country_Object)
            {
                return NotFound();
            }
            else
            {
                CountryDto CountryDto_Object = Country_Object.Adapt<CountryDto>();
                return Ok(CountryDto_Object);
            }
        }

        // POST: api/Country
        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromBody] CountryForSaveDto CountryDto_Object,
                                                        string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsSaved = 0;
                
                if (!ModelState.IsValid)
                {
                    _logger.LogError($"ModelState is Invalid for {UserName} in action CreateCountry");
                    return BadRequest(ModelState);
                }

                Country Country_Object = CountryDto_Object.Adapt<Country>();

                await _repositoryWrapper.CountryRepositoryWrapper.Create(Country_Object);
                NumberOfObjectsSaved = await _repositoryWrapper.Save();

#if Use_Hub_Logic_On_ServerSide
        await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
                if (1 == NumberOfObjectsSaved)
                {
                    _logger.LogInfo($"Country with Id : {Country_Object.CountryID} has been stored by {UserName} !!!");
                    return Ok(Country_Object.CountryID);
                }
                else
                {
                    _logger.LogError($"Error when saving Country by {UserName} !!!");
                    return BadRequest($"Error when saving Country by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside Save Country action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error for {UserName}");
            }
        }

        // PUT: api/Country/5
        [HttpPut("UpdateCountry/{CountryId}")]
        public async Task<IActionResult> UpdateCountry(int CountryId,
                                                    [FromBody] CountryForUpdateDto CountryForUpdateDto_Object,
                                                    string UserName = "No Name")
        {
            int NumberOfObjectsUpdated = 0;
            try
            {

                if (CountryId != CountryForUpdateDto_Object.CountryID)
                {
                    _logger.LogError($"CountryId !=  CityForUpdateDto_Object.CountryId for {UserName} in action UpdateCountry");
                    return BadRequest($"CountryId !=  CityForUpdateDto_Object.CountryId for {UserName} in action UpdateCountry");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"ModelState is Invalid for {UserName} in action UpdateCountry");
                    return BadRequest(ModelState);
                }

                Country CountryFromRepo = await _repositoryWrapper.CountryRepositoryWrapper.FindOne(CountryId);

                if (null == CountryFromRepo)
                {
                    _logger.LogError($"CountryId {CountryId} not found in database for {UserName} in action UpdateCountry");
                    return NotFound();
                }

                TypeAdapter.Adapt(CountryForUpdateDto_Object, CountryFromRepo);

                await _repositoryWrapper.CountryRepositoryWrapper.Update(CountryFromRepo);

                NumberOfObjectsUpdated = await _repositoryWrapper.Save();

                if (1 == NumberOfObjectsUpdated)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCountryDataMessage");
#endif
                    _logger.LogInfo($"Country with Id : {CountryFromRepo.CountryID} has been updated by {UserName} !!!");
                    return Ok($"Country with Id : {CountryFromRepo.CountryID} has been updated by {UserName} !!!"); ;
                }
                else
                {
                    _logger.LogError($"Error when updating Country with Id : {CountryFromRepo.CountryID} by {UserName} !!!");
                    return BadRequest($"Error when updating Country with Id : {CountryFromRepo.CountryID} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside Update Country action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error for {UserName}");
            }
        }

        // DELETE: api/5
        [HttpDelete("DeleteCountry/{CountryId}")]
        public async Task<IActionResult> DeleteCountry(int CountryId,
                                                       string UserName = "No Name")
        {
            int NumberOfObjectsDeleted;

            Country CountryFromRepo = await _repositoryWrapper.CountryRepositoryWrapper.FindOne(CountryId);

            if (null == CountryFromRepo)
            {
                _logger.LogError($"Country with Id {CountryId} not found inside action DeleteCountry for {UserName}");
                return NotFound();
            }

            await _repositoryWrapper.CountryRepositoryWrapper.Delete(CountryFromRepo);

            NumberOfObjectsDeleted = await _repositoryWrapper.Save();

            if (1 == NumberOfObjectsDeleted)
            {
#if Use_Hub_Logic_On_ServerSide
                await this._broadcastHub.Clients.All.SendAsync("UpdateCountryDataMessage");
#endif
                _logger.LogInfo($"Country with Id {CountryId} has been deleted in action DeleteCountry by {UserName}");
                return Ok($"Country with Id {CountryId} has been deleted in action DeleteCountry by {UserName}");
            }
            else
            {
                _logger.LogError($"Error when deleting Country with Id : {CountryFromRepo.CountryID} by {UserName} !!!");
                return BadRequest($"Error when deleting Country with Id : {CountryFromRepo.CountryID} by {UserName} !!!");
            }
        }
    }
}
