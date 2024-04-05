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
        private ICityService _cityService;

#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif
        public CityController(ILoggerManager logger,
                              IRepositoryWrapper repositoryWrapper,
                              ICityService cityService)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
            this._cityService = cityService;
        }

        [HttpGet("GetCities")]
        public async Task<IActionResult> GetCities(string UserName = "No Name")
        {
            try
            {
                IEnumerable<City> CityList = new List<City>();

                _repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();
                CityList = await _repositoryWrapper.CityRepositoryWrapper.FindAll();

                List<CityDto> CityDtos;

                CityDtos = CityList.Adapt<CityDto[]>().ToList();

                _logger.LogInfo($"All Cities has been read from GetCities action by {UserName}");
                return Ok(CityDtos);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCities action for {UserName} : {Error.Message}");
                return StatusCode(500, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetCity/{CityId}")]
        public async Task<IActionResult> GetCity(int CityId,
                                                 string UserName = "No Name")
        {
            _repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();

            City City_Object = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

            if (null == City_Object)
            {
                return NotFound();
            }
            else
            {
                CityDto CityDto_Object = City_Object.Adapt<CityDto>();
                return Ok(CityDto_Object);
            }
        }

        // POST: api/City
        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityForSaveWithCountryDto CityDto_Object,
                                                    string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsSaved = 0;
                if (CityDto_Object.CityDescription == CityDto_Object.CityName)
                {
                    ModelState.AddModelError(
                        "Description",
                        "The provided description should be different from the name.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"ModelState is Invalid for {UserName} in action CreateCity");
                    return BadRequest(ModelState);
                }

                City City_Object = CityDto_Object.Adapt<City>();

                await _repositoryWrapper.CityRepositoryWrapper.Create(City_Object);
                NumberOfObjectsSaved = await _repositoryWrapper.Save();

#if Use_Hub_Logic_On_ServerSide
        await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
                if (1 == NumberOfObjectsSaved)
                {
                    _logger.LogInfo($"City with Id : {City_Object.CityId} has been stored by {UserName} !!!");
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
                _logger.LogError($"Something went wrong inside Save City action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error for {UserName}");
            }
        }

        // PUT: api/City/5
        [HttpPut("UpdateCity/{CityId}")]
        public async Task<IActionResult> UpdateCity(int CityId,
                                                    [FromBody] CityForUpdateDto CityForUpdateDto_Object,
                                                    string UserName = "No Name")
        {
            int NumberOfObjectsUpdated = 0;
            try
            {

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

                City CityFromRepo = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

                if (null == CityFromRepo)
                {
                    return NotFound();
                }

                TypeAdapter.Adapt(CityForUpdateDto_Object, CityFromRepo);

                await _repositoryWrapper.CityRepositoryWrapper.Update(CityFromRepo);

                //NumberOfObjectsUpdated = await _repositoryWrapper.CityRepositoryWrapper.Save();
                NumberOfObjectsUpdated = await _repositoryWrapper.Save();

                if (1 == NumberOfObjectsUpdated)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
                    _logger.LogInfo($"City with Id : {CityFromRepo.CityId} has been updated by {UserName} !!!");
                    return Ok($"City with Id : {CityFromRepo.CityId} has been updated by {UserName} !!!"); ;
                }
                else
                {
                    _logger.LogError($"Error when updating City with Id : {CityFromRepo.CityId} by {UserName} !!!");
                    return BadRequest($"Error when updating City with Id : {CityFromRepo.CityId} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside Update City action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error for {UserName}");
            }
        }

        // DELETE: api/5
        [HttpDelete("DeleteCity/{CityId}")]
        public async Task<IActionResult> DeleteCity(int CityId,
                                                    string UserName = "No Name")
        {
            int NumberOfObjectsDeleted;

            _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();

            var CityFromRepo = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

            if (null == CityFromRepo)
            {
                _logger.LogError($"City with Id {CityId} not found inside action DeleteCity for {UserName}");
                return NotFound();
            }

            await _repositoryWrapper.CityRepositoryWrapper.Delete(CityFromRepo);

            //NumberOfObjectsDeleted = await _repositoryWrapper.CityRepositoryWrapper.Save();
            NumberOfObjectsDeleted = await _repositoryWrapper.Save();

            if (1 == NumberOfObjectsDeleted)
            {
#if Use_Hub_Logic_On_ServerSide
                await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
                _logger.LogInfo($"City with Id {CityId} has been deleted in action DeleteCity for {UserName}");
                return Ok($"City with Id {CityId} has been deleted in action DeleteCity for {UserName}");
            }
            else
            {
                _logger.LogError($"Error when deleting City with Id : {CityFromRepo.CityId} by {UserName} !!!");
                return BadRequest($"Error when deleting City with Id : {CityFromRepo.CityId} by {UserName} !!!");
            }
        }
    }
}
