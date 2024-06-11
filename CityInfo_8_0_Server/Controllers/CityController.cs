using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ServicesContracts;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo_8_0_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;

#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif
        public CityController(ILoggerManager logger,
                              IRepositoryWrapper repositoryWrapper)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("GetCities")]
        public async Task<IActionResult> GetCities(string UserName = "No Name")
        {
            try
            {
                IEnumerable<City> CityList = new List<City>();

                this._repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();
                CityList = await this._repositoryWrapper.CityRepositoryWrapper.FindAll();

                List<CityDto> CityDtos;

                CityDtos = CityList.Adapt<CityDto[]>().ToList();

                this._logger.LogInfo($"All Cities has been read from GetCities action by {UserName}");
                return Ok(CityDtos);
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside GetCities action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetCity/{CityId}")]
        public async Task<IActionResult> GetCity(int CityId,
                                                 string UserName = "No Name")
        {
            try
            {
                this._repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();

                City City_Object = await this._repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

                if (null == City_Object)
                {
                    return NotFound();
                }
                else
                {
                    CityDto CityDto_Object = City_Object.Adapt<CityDto>();
                    this._logger.LogInfo($"City with CityId {CityId} has been read from GetCity action by {UserName}");
                    return Ok(CityDto_Object);
                }
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside GetCity action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetCitiesInCountry/{CountryId}")]
        public async Task<IActionResult> GetCitiesInCountry(int CountryId,
                                                            string UserName = "No Name")
        {
            try
            {
              IEnumerable<City> CityList = new List<City>();

              this._repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();

              CityList = await this._repositoryWrapper.CityRepositoryWrapper.GetCitiesInCountry(CountryId);

              List<CityDto> CityDtos;

              CityDtos = CityList.Adapt<CityDto[]>().ToList();

              this._logger.LogInfo($"All Cities within Country Id : {CountryId} has been read from GetCitiesInCountry action by {UserName}");
              return Ok(CityDtos);
            }
            catch (Exception Error)
            {
              this._logger.LogError($"Something went wrong inside GetCitiesInCountry action for {UserName} : {Error.Message}");
              return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // POST: api/City
        [HttpPost("CreateCity")]
        public async Task<IActionResult> CreateCity([FromBody] CityForSaveDto CityForSaveDto_Object,
                                                    string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsSaved = 0;
                if (CityForSaveDto_Object.CityDescription == CityForSaveDto_Object.CityName)
                {
                    ModelState.AddModelError(
                        "Description",
                        "The provided description should be different from the name.");
                }

                if (!ModelState.IsValid)
                {
                    this._logger.LogError($"ModelState is Invalid for {UserName} in action CreateCity");
                    return BadRequest(ModelState);
                }

                City City_Object = CityForSaveDto_Object.Adapt<City>();

                await this._repositoryWrapper.CityRepositoryWrapper.Create(City_Object);
                NumberOfObjectsSaved = await this._repositoryWrapper.Save();

                if (1 == NumberOfObjectsSaved)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
                    _logger.LogInfo($"City with Id : {City_Object.CityId} has been saved by {UserName} !!!");
                    return Ok(City_Object.CityId);
                }
                else
                {
                    _logger.LogError($"Error when saving City by {UserName} !!!");
                    return BadRequest($"Error when saving City by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside CreateCity action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // PUT: api/City/5
        [HttpPut("UpdateCity/{CityId}")]
        public async Task<IActionResult> UpdateCity(int CityId,
                                                    [FromBody] CityForUpdateDto CityForUpdateDto_Object,
                                                    string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsUpdated = 0;

                if (CityId != CityForUpdateDto_Object.CityId)
                {
                    _logger.LogError($"CityId !=  CityForUpdateDto_Object.CityId for {UserName} in action UpdateCity");
                    return BadRequest($"CityId !=  CityForUpdateDto_Object.CityId for {UserName} in action UpdateCity");
                }

                if (CityForUpdateDto_Object.CityDescription == CityForUpdateDto_Object.CityName)
                {
                    ModelState.AddModelError(
                        "Description",
                        "The provided description should be different from the name.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"ModelState is Invalid for {UserName} in action UpdateCity");
                    return BadRequest(ModelState);
                }

                City City_Object = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

                if (null == City_Object)
                {
                    return NotFound();
                }

                TypeAdapter.Adapt(CityForUpdateDto_Object, City_Object);

                await _repositoryWrapper.CityRepositoryWrapper.Update(City_Object);

                NumberOfObjectsUpdated = await _repositoryWrapper.Save();

                if (1 == NumberOfObjectsUpdated)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
                    _logger.LogInfo($"City with Id : {City_Object.CityId} has been updated by {UserName} !!!");
                    return Ok($"City with Id : {City_Object.CityId} has been updated by {UserName} !!!"); ;
                }
                else
                {
                    _logger.LogError($"Error when updating City with Id : {City_Object.CityId} by {UserName} !!!");
                    return BadRequest($"Error when updating City with Id : {City_Object.CityId} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside UpdateCity action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // DELETE: api/City/5
        [HttpDelete("DeleteCity/{CityId}")]
        public async Task<IActionResult> DeleteCity(int CityId,
                                                    string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsDeleted;

                City City_Object = await this._repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

                if (null == City_Object)
                {
                    this._logger.LogError($"City with Id {CityId} not found inside action DeleteCity for {UserName}");
                    return NotFound();
                }

                await this._repositoryWrapper.CityRepositoryWrapper.Delete(City_Object);

                NumberOfObjectsDeleted = await this._repositoryWrapper.Save();

                if (1 == NumberOfObjectsDeleted)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
                    this._logger.LogInfo($"City with Id {CityId} has been deleted in action DeleteCity by {UserName}");
                    return Ok($"City with Id {CityId} has been deleted in action DeleteCity by {UserName}");
                }
                else
                {
                    _logger.LogError($"Error when deleting City with Id : {City_Object.CityId} by {UserName} !!!");
                    return BadRequest($"Error when deleting City with Id : {City_Object.CityId} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside DeleteCity action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }
    }
}
