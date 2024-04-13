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
    public class CityLanguageController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;

#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif

        public CityLanguageController(ILoggerManager logger,
                                      IRepositoryWrapper repositoryWrapper)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("GetCiyLanguages")]
        public async Task<IActionResult> GetCiyLanguages(string UserName = "No Name")
        {
            try
            {
                IEnumerable<CityLanguage> CityLanguageList = new List<CityLanguage>();

                _repositoryWrapper.CityLanguageRepositoryWrapper.EnableLazyLoading();
                CityLanguageList = await _repositoryWrapper.CityLanguageRepositoryWrapper.FindAll();

                List<CityLanguageDto> CityLanguageDtos;

                CityLanguageDtos = CityLanguageList.Adapt<CityLanguageDto[]>().ToList();

                _logger.LogInfo($"All CityLangueges has been read from GetCiyLanguages action by {UserName}");
                return Ok(CityLanguageDtos);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCiyLanguages action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetLanguagesWithCityId/{CityId}")]
        public async Task<IActionResult> GetLanguagesWithCityId(int CityId,
                                                                string UserName = "No Name")
        {
            try
            {
                _repositoryWrapper.CityLanguageRepositoryWrapper.EnableLazyLoading();

                IEnumerable<CityLanguage> CityLanguageList = new List<CityLanguage>();
                CityLanguageList = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetAllLanguagesWithCityId(CityId);

                if (null == CityLanguageList)
                {
                    return NotFound();
                }
                else
                {
                    List<CityLanguageDto> CityLanguageDtos;

                    CityLanguageDtos = CityLanguageList.Adapt<CityLanguageDto[]>().ToList();

                    return Ok(CityLanguageDtos);
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetLanguagesWithCityId action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetCitiesWithLanguageId/{languageId}")]
        public async Task<IActionResult> GetCitiesWithLanguageId(int languageId,
                                                                 string UserName = "No Name")
        {
            try
            {
                _repositoryWrapper.CityLanguageRepositoryWrapper.EnableLazyLoading();

                IEnumerable<CityLanguage> CityLanguageList = new List<CityLanguage>();
                CityLanguageList = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetAllCitiesFromLanguageId(languageId);

                if (null == CityLanguageList)
                {
                    return NotFound();
                }
                else
                {
                    List<CityLanguageDto> CityLanguageDtos;

                    CityLanguageDtos = CityLanguageList.Adapt<CityLanguageDto[]>().ToList();

                    return Ok(CityLanguageDtos);
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCitiesWithLanguageId action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetCityLanguageWithLanguageIdAndCityId/{CityId}/{LanguageId}")]
        public async Task<IActionResult> GetCityLanguageWithLanguageIdAndCityId([FromRoute] int CityId,
                                                                                [FromRoute] int LanguageId,
                                                                                string UserName = "No Name")
        {
            try
            {
                _repositoryWrapper.CityLanguageRepositoryWrapper.EnableLazyLoading();

                CityLanguage CityLanguage_Object = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetCityIdLanguageIdCombination(CityId, LanguageId);

                if (null == CityLanguage_Object)
                {
                    return NotFound();
                }
                else
                {
                    CityLanguageDto CityLanguageDto_Object = CityLanguage_Object.Adapt<CityLanguageDto>();

                    return Ok(CityLanguageDto_Object);
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCityLanguageWithLanguageIdAndCityId action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // POST: api/CityLanguage
        [HttpPost]
        public async Task<IActionResult> CreateCityLanguage([FromBody] CityLanguageForSaveDto CityLanguageForSaveDto_Object,
                                                            string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsSaved = 0;

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"ModelState is Invalid for {UserName} in action CreateCityLanguage");
                    return BadRequest(ModelState);
                }

                CityLanguage CityLanguage_Object = CityLanguageForSaveDto_Object.Adapt<CityLanguage>();

                await _repositoryWrapper.CityLanguageRepositoryWrapper.Create(CityLanguage_Object);
                NumberOfObjectsSaved = await _repositoryWrapper.Save();

                if (1 == NumberOfObjectsSaved)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCityLanguageDataMessage");
#endif
                    _logger.LogInfo($"CityLanguage with CityId : {CityLanguage_Object.CityId} and LanguageId {CityLanguage_Object.LanguageId} has been stored by {UserName} !!!");
                    return Ok(CityLanguageForSaveDto_Object);
                }
                else
                {
                    _logger.LogError($"Error when saving CityLanguage with CityId : {CityLanguage_Object.CityId} and LanguageId {CityLanguage_Object.LanguageId} by {UserName} !!!");
                    return BadRequest($"Error when saving CityLanguage with CityId : {CityLanguage_Object.CityId} and LanguageId {CityLanguage_Object.LanguageId} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside CreateCityLanguage action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error for {UserName}");
            }
        }

        // PUT: api/City/5
        [HttpPut("UpdateCityLanguage/{OldCityId}/{OldLanguageId}")]
        public async Task<IActionResult> UpdateCityLanguage(int OldCityId,
                                                            int OldLanguageId,
                                                            [FromBody] CityLanguageForUpdateDto CityLanguageForUpdateDto_Object,
                                                            string UserName = "No Name")
        {
            using var Transaction = await _repositoryWrapper.GetCurrentDatabaseContext().Database.BeginTransactionAsync();

            //{
            try
            {
                int NumberOfObjectsUpdated = 0;
                int NumberOfObjectsDeleted = 0;

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"ModelState is Invalid for {UserName} in action UpdateCity");
                    return BadRequest(ModelState);
                }

                await Transaction.CreateSavepointAsync("BeforeUpdate");

                CityLanguage CityLanguage_Object = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetCityIdLanguageIdCombination(OldCityId, OldLanguageId);

                if (null == CityLanguage_Object)
                {
                    return NotFound();
                }
                else
                {
                    await _repositoryWrapper.CityLanguageRepositoryWrapper.Delete(CityLanguage_Object);

                    NumberOfObjectsDeleted = await _repositoryWrapper.Save();

                    if (1 == NumberOfObjectsDeleted)
                    {
                        TypeAdapter.Adapt(CityLanguageForUpdateDto_Object, CityLanguage_Object);

                        await _repositoryWrapper.CityLanguageRepositoryWrapper.Update(CityLanguage_Object);

                        NumberOfObjectsUpdated = await _repositoryWrapper.Save();

                        if (1 == NumberOfObjectsUpdated)
                        {


#if Use_Hub_Logic_On_ServerSide
                        await this._broadcastHub.Clients.All.SendAsync("UpdateCityLanguageDataMessage");
#endif
                            await Transaction.RollbackToSavepointAsync("BeforeUpdate");
                            _logger.LogInfo($"CityLanguage with CityId : {CityLanguageForUpdateDto_Object} and LanguageId : {CityLanguageForUpdateDto_Object.LanguageId} has been updated to {CityLanguage_Object.CityId} and LanguageId {CityLanguage_Object.LanguageId} by {UserName} !!!");
                            return Ok(CityLanguage_Object);
                        }
                        else
                        {
                            await Transaction.RollbackToSavepointAsync("BeforeUpdate");
                            _logger.LogError($"Error when updating CityLanguage with CityId : {CityLanguage_Object.CityId} and LanguageId {CityLanguage_Object.LanguageId} by {UserName} !!!");
                            return BadRequest($"Error when updating CityLanguage with CityId : {CityLanguage_Object.CityId} and LanguageId {CityLanguage_Object.LanguageId} by {UserName} !!!");
                        }

                    }
                    else
                    {
                        await Transaction.RollbackToSavepointAsync("BeforeUpdate");
                        _logger.LogError($"Error when deleting CityLanguage with CityId : {CityLanguage_Object.CityId} and LanguageId {CityLanguage_Object.LanguageId} by {UserName} !!!");
                        return BadRequest($"Error when deleting CityLanguage with CityId : {CityLanguage_Object.CityId} and LanguageId {CityLanguage_Object.LanguageId} by {UserName} !!!");
                    }
                }
            }
            catch (Exception Error)
            {
                await Transaction.RollbackToSavepointAsync("BeforeUpdate");
                _logger.LogError($"Something went wrong inside UpdateCity action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error for {UserName}");
            }
            //}
        }
    }
}
