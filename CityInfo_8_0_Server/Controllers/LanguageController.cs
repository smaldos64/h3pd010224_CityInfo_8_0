using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo_8_0_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;

#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif
        public LanguageController(ILoggerManager logger,
                                  IRepositoryWrapper repositoryWrapper)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
        }

        // GET: api/<LanguageController>
        [HttpGet("GetLanguages")]
        public async Task<IActionResult> GetLanguages(string UserName = "No Name")
        {
            try
            {
                IEnumerable<Language> LanguageList = new List<Language>();

                this._repositoryWrapper.LanguageRepositoryWrapper.EnableLazyLoading();
                LanguageList = await this._repositoryWrapper.LanguageRepositoryWrapper.FindAll();

                List<LanguageDto> LanguageDtos;

                LanguageDtos = LanguageList.Adapt<LanguageDto[]>().ToList();

                this._logger.LogInfo($"All Languages has been read from GetLanguages action by {UserName}");
                return Ok(LanguageDtos);
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside GetLanguages action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetLanguage/{LanguageId}")]
        public async Task<IActionResult> GetLanguage(int LanguageId,
                                                     string UserName = "No Name")
        {
            try
            {
                this._repositoryWrapper.LanguageRepositoryWrapper.EnableLazyLoading();

                Language Language_Object = await this._repositoryWrapper.LanguageRepositoryWrapper.FindOne(LanguageId);

                if (null == Language_Object)
                {
                    return NotFound();
                }
                else
                {
                    LanguageDto LanguageDto_Object = Language_Object.Adapt<LanguageDto>();
                    return Ok(LanguageDto_Object);
                }
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside GetLanguage action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // POST: api/Language
        [HttpPost("CreateLanguage")]
        public async Task<IActionResult> CreateLanguage([FromBody] LanguageForSaveDto LanguageForSaveDto_Object,
                                                        string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsSaved = 0;
                
                Language Language_Object = LanguageForSaveDto_Object.Adapt<Language>();

                await this._repositoryWrapper.LanguageRepositoryWrapper.Create(Language_Object);
                NumberOfObjectsSaved = await this._repositoryWrapper.Save();

#if Use_Hub_Logic_On_ServerSide
                await this._broadcastHub.Clients.All.SendAsync("UpdateLanguageDataMessage");
#endif
                if (1 == NumberOfObjectsSaved)
                {
                    this._logger.LogInfo($"Language with Id : {Language_Object.LanguageId} has been stored by {UserName} !!!");
                    return Ok(Language_Object.LanguageId);
                }
                else
                {
                    this._logger.LogError($"Error when saving Language by {UserName} !!!");
                    return BadRequest($"Error when saving Language by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside CreateLanguage action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // PUT: api/Language/5
        [HttpPut("UpdateLanguage/{LanguageId}")]
        public async Task<IActionResult> UpdateLanguage(int LanguageId,
                                                       [FromBody] LanguageForUpdateDto LanguageForUpdateDto_Object,
                                                       string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsUpdated = 0;

                if (LanguageId != LanguageForUpdateDto_Object.LanguageId)
                {
                    this._logger.LogError($"LanguageId !=  LanguageForUpdateDto_Object.LanguageId for {UserName} in action UpdateLanguage");
                    return BadRequest($"LanguageId !=  LanguageForUpdateDto_Object.LanguageId for {UserName} in action UpdateLanguage");
                }

                if (!ModelState.IsValid)
                {
                    this._logger.LogError($"ModelState is Invalid for {UserName} in action UpdateLanguage");
                    return BadRequest(ModelState);
                }

                Language Language_Object = await this._repositoryWrapper.LanguageRepositoryWrapper.FindOne(LanguageId);

                if (null == Language_Object)
                {
                    return NotFound();
                }

                TypeAdapter.Adapt(LanguageForUpdateDto_Object, Language_Object);

                await this._repositoryWrapper.LanguageRepositoryWrapper.Update(Language_Object);

                NumberOfObjectsUpdated = await this._repositoryWrapper.Save();

                if (1 == NumberOfObjectsUpdated)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateLanguageDataMessage");
#endif
                    this._logger.LogInfo($"City with Id : {Language_Object.LanguageId} has been updated by {UserName} !!!");
                    return Ok($"City with Id : {Language_Object.LanguageId} has been updated by {UserName} !!!"); ;
                }
                else
                {
                    this._logger.LogError($"Error when updating Language with Id : {Language_Object.LanguageId} by {UserName} !!!");
                    return BadRequest($"Error when updating Language with Id : {Language_Object.LanguageId} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside UpdateLanguage action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // DELETE: api/Language/5
        [HttpDelete("DeleteLanguage/{LanguageId}")]
        public async Task<IActionResult> DeleteLanguage(int LanguageId,
                                                        string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsDeleted;

                Language Language_Object = await this._repositoryWrapper.LanguageRepositoryWrapper.FindOne(LanguageId);

                if (null == Language_Object)
                {
                    this._logger.LogError($"Language with Id {LanguageId} not found inside action DeleteLanguage for {UserName}");
                    return NotFound();
                }

                await this._repositoryWrapper.LanguageRepositoryWrapper.Delete(Language_Object);

                NumberOfObjectsDeleted = await this._repositoryWrapper.Save();

                if (1 == NumberOfObjectsDeleted)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateLanguageDataMessage");
#endif
                    this._logger.LogInfo($"Language with Id {LanguageId} has been deleted in action DeleteLanguage by {UserName}");
                    return Ok($"Language with Id {LanguageId} has been deleted in action DeleteLanguage by {UserName}");
                }
                else
                {
                    this._logger.LogError($"Error when deleting Language with Id : {Language_Object.LanguageId} by {UserName} !!!");
                    return BadRequest($"Error when deleting Language with Id : {Language_Object.LanguageId} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside DeleteLanguage action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }
    }
}
