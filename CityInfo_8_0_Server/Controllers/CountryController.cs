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

                this._repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();
                CountryList = await this._repositoryWrapper.CountryRepositoryWrapper.FindAll();

                List<CountryDto> CountryDtos;

                CountryDtos = CountryList.Adapt<CountryDto[]>().ToList();

                this._logger.LogInfo($"All Countries has been read from GetCountries action by {UserName}");
                return Ok(CountryDtos);
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside GetCountries action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetCountry/{CountryId}")]
        public async Task<IActionResult> GetCountry(int CountryId,
                                                    string UserName = "No Name")
        {
            try
            {
                this._repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();

                Country Country_Object = await this._repositoryWrapper.CountryRepositoryWrapper.FindOne(CountryId);

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
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCountry action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // POST: api/Country
        [HttpPost("CreateCountry")]
        public async Task<IActionResult> CreateCountry([FromBody] CountryForSaveDto CountryDto_Object,
                                                        string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsSaved = 0;

                Country Country_Object = CountryDto_Object.Adapt<Country>();

                await this._repositoryWrapper.CountryRepositoryWrapper.Create(Country_Object);
                NumberOfObjectsSaved = await this._repositoryWrapper.Save();

#if Use_Hub_Logic_On_ServerSide
                await this._broadcastHub.Clients.All.SendAsync("UpdateCountryDataMessage");
#endif
                if (1 == NumberOfObjectsSaved)
                {
                    this._logger.LogInfo($"Country with Id : {Country_Object.CountryID} has been stored by {UserName} !!!");
                    return Ok(Country_Object.CountryID);
                }
                else
                {
                    this._logger.LogError($"Error when saving Country by {UserName} !!!");
                    return BadRequest($"Error when saving Country by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside CreateCountry action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
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
                    this._logger.LogError($"CountryId !=  CityForUpdateDto_Object.CountryId for {UserName} in action UpdateCountry");
                    return BadRequest($"CountryId !=  CityForUpdateDto_Object.CountryId for {UserName} in action UpdateCountry");
                }

                if (!ModelState.IsValid)
                {
                    this._logger.LogError($"ModelState is Invalid for {UserName} in action UpdateCountry");
                    return BadRequest(ModelState);
                }

                Country Country_Object = await this._repositoryWrapper.CountryRepositoryWrapper.FindOne(CountryId);

                if (null == Country_Object)
                {
                    this._logger.LogError($"CountryId {CountryId} not found in database for {UserName} in action UpdateCountry");
                    return NotFound();
                }

                TypeAdapter.Adapt(CountryForUpdateDto_Object, Country_Object);

                await this._repositoryWrapper.CountryRepositoryWrapper.Update(Country_Object);

                NumberOfObjectsUpdated = await this._repositoryWrapper.Save();

                if (1 == NumberOfObjectsUpdated)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCountryDataMessage");
#endif
                    this._logger.LogInfo($"Country with Id : {Country_Object.CountryID} has been updated by {UserName} !!!");
                    return Ok($"Country with Id : {Country_Object.CountryID} has been updated by {UserName} !!!"); ;
                }
                else
                {
                    this._logger.LogError($"Error when updating Country with Id : {Country_Object.CountryID} by {UserName} !!!");
                    return BadRequest($"Error when updating Country with Id : {Country_Object.CountryID} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside Update Country action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // DELETE: api/Country/5
        [HttpDelete("DeleteCountry/{CountryId}")]
        public async Task<IActionResult> DeleteCountry(int CountryId,
                                                       string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsDeleted;

                Country CountryFromRepo = await this._repositoryWrapper.CountryRepositoryWrapper.FindOne(CountryId);

                if (null == CountryFromRepo)
                {
                    this._logger.LogError($"Country with Id {CountryId} not found inside action DeleteCountry for {UserName}");
                    return NotFound();
                }

                await this._repositoryWrapper.CountryRepositoryWrapper.Delete(CountryFromRepo);

                NumberOfObjectsDeleted = await this._repositoryWrapper.Save();

                if (1 == NumberOfObjectsDeleted)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCountryDataMessage");
#endif
                    this._logger.LogInfo($"Country with Id {CountryId} has been deleted in action DeleteCountry by {UserName}");
                    return Ok($"Country with Id {CountryId} has been deleted in action DeleteCountry by {UserName}");
                }
                else
                {
                    _logger.LogError($"Error when deleting Country with Id : {CountryFromRepo.CountryID} by {UserName} !!!");
                    return BadRequest($"Error when deleting Country with Id : {CountryFromRepo.CountryID} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside DeleteCountry action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }
    }
}
