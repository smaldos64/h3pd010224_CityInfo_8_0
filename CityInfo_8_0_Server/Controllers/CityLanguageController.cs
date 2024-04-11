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
    }
}
