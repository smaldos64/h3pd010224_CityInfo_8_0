using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Mapster;
using Microsoft.Extensions.Logging;
using Contracts;
using ServicesContracts;
using Entities.DataTransferObjects;
using Services;

#if Use_Hub_Logic_On_ServerSide
using CityInfo_8_0_Server.Hubs;
#endif


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo_8_0_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityLanguageEducationController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;
        private ICityLanguageService _cityLanguageService;

#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif

        public CityLanguageEducationController(ILoggerManager logger,
                                      IRepositoryWrapper repositoryWrapper,
                                      ICityLanguageService cityLanguageService)
        {
          this._logger = logger;
          this._repositoryWrapper = repositoryWrapper;
          this._cityLanguageService = cityLanguageService;
        }

        // GET: api/<CityLanguageController>
        //        [HttpGet]
        //        public async Task<ActionResult<IEnumerable<CityLanguageDto>>> GetCityLanguages(bool includeRelations = true,
        //                                                                                       string UserName = "No Name")
        //        {
        //            if (false == includeRelations)
        //            {
        //                _repositoryWrapper.LanguageRepositoryWrapper.DisableLazyLoading();
        //            }
        //            else  // true == includeRelations 
        //            {
        //                _repositoryWrapper.LanguageRepositoryWrapper.EnableLazyLoading();
        //            }

        //            var CityLanguageList = await _repositoryWrapper.CityLanguageRepositoryWrapper.FindAll();

        //            List<CityLanguageDto> CityLanguageDtos = CityLanguageList.Adapt<CityLanguageDto[]>().ToList();

        //#if Test_Logging
        //            this._logger.LogWarning(CityLanguageControllerLoggingEventID, "Alle Byer-Sprog relationer læst af : " + UserName);
        //#endif

        //            return Ok(CityLanguageDtos.ToList());
        //        }

        //        [HttpGet("{CityId}")]
        //        public async Task<ActionResult<IList<CityLanguageDto>>> GetLanguagesForCity(int CityId,
        //                                                                                    bool includeRelations = true,
        //                                                                                    string UserName = "No Name")
        //        {
        //            if (false == includeRelations)
        //            {
        //                _repositoryWrapper.LanguageRepositoryWrapper.DisableLazyLoading();
        //            }
        //            else  // true == includeRelations 
        //            {
        //                _repositoryWrapper.LanguageRepositoryWrapper.EnableLazyLoading();
        //            }

        //            var CityLanguageList = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetAllLanguagesFromCityID(CityId);

        //            List<CityLanguageDto> CityLanguageDtos = CityLanguageList.Adapt<CityLanguageDto[]>().ToList();

        //#if Test_Logging
        //            this._logger.LogWarning(CityLanguageControllerLoggingEventID, "Alle Sprog til CityID : " + CityId.ToString() + " læst af : " + UserName);
        //#endif

        //            return Ok(CityLanguageDtos.ToList());
        //        }

        //        // GET api/<CityLanguageController>/5/4
        //        [HttpGet("{CityId}/{LanguageId}")]
        //        public async Task<IActionResult> GetCityLanguage(int CityId, 
        //                                                         int LanguageId,  
        //                                                         bool includeRelations = true,
        //                                                         string UserName = "No Name")
        //        {
        //            if (false == includeRelations)
        //            {
        //                _repositoryWrapper.CityLanguageRepositoryWrapper.DisableLazyLoading();
        //            }
        //            else
        //            {
        //                _repositoryWrapper.CityLanguageRepositoryWrapper.EnableLazyLoading();
        //            }

        //            var CityLanguage_Object = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetCityIdLanguageIdCombination(CityId, LanguageId);

        //            if (null == CityLanguage_Object)
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //#if Test_Logging
        //                this._logger.LogWarning(CityLanguageControllerLoggingEventID, "By-Sprog relation med CityId : " + CityId.ToString() + " og languageId : " + LanguageId + " læst af : " + UserName);
        //#endif
        //                CityLanguageDto CityLanguageDto_Object = CityLanguage_Object.Adapt<CityLanguageDto>();
        //                return Ok(CityLanguageDto_Object);
        //            }
        //        }

        //        // POST api/<CityLanguageController>
        //        [HttpPost]
        //        public async Task<IActionResult> CreateCityLanguage([FromBody] CityLanguageForSaveAndUpdateDto CityLanguageDto_Object,
        //                                                            string UserName = "No Name")
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return BadRequest(ModelState);
        //            }

        //            //CityLanguage Citylanguage_Object = CityLanguageDto_Object.Adapt<CityLanguage>();

        //            CityLanguage CityLanguage_Object = new CityLanguage();

        //            if (CityLanguage_Object.CloneData<CityLanguage>(CityLanguageDto_Object))
        //            {
        //                try
        //                {
        //                    await _repositoryWrapper.CityLanguageRepositoryWrapper.Create(CityLanguage_Object);
        //#if Test_Logging
        //                    this._logger.LogWarning(CityLanguageControllerLoggingEventID, "By-Sprog relation med CityId : " 
        //                        + CityLanguageDto_Object.CityId.ToString() + " og languageId : " 
        //                        + CityLanguageDto_Object.LanguageId.ToString() + " oprettet af : " + UserName);
        //#endif

        //#if Use_Hub_Logic_On_ServerSide
        //                    await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
        //#endif
        //                    return Ok(CityLanguageDto_Object);
        //                }
        //                catch (Exception Error)
        //                {
        //                    return BadRequest(Error.ToString());
        //                }
        //            }
        //            else
        //            {
        //                return BadRequest();
        //            }
        //        }

        //        // PUT api/<CityLanguageController>/5/4
        //        //[HttpPut("{CityId, LanguageId}")]
        //        [HttpPut("{CityId}/{LanguageId}")]
        //        public async Task<IActionResult> UpdateCityLanguage(int CityId, 
        //                                                            int LanguageId, 
        //                                                            [FromBody] CityLanguageForSaveAndUpdateDto CityLanguageDto_Object,
        //                                                            string UserName = "No Name")
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return BadRequest(ModelState);
        //            }

        //            CityLanguage CityLanguageFromRepoDelete = new CityLanguage();

        //            try
        //            {
        //                CityLanguageFromRepoDelete = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetCityIdLanguageIdCombination(CityId, LanguageId);
        //            }
        //            catch (Exception Error)
        //            {
        //                return NotFound();
        //            }

        //            if (null == CityLanguageFromRepoDelete)
        //            {
        //                return NotFound();
        //            }

        //            CityLanguage CityLanguageToSave = new CityLanguage();

        //            if (true == CityLanguageToSave.CloneData<CityLanguage>(CityLanguageDto_Object))
        //            {
        //               await _repositoryWrapper.CityLanguageRepositoryWrapper.UpdateCityLanguageCombination(CityLanguageFromRepoDelete,
        //                                                                                                    CityLanguageToSave);
        //#if Use_Hub_Logic_On_ServerSide
        //                await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
        //#endif

        //#if Test_Logging
        //                this._logger.LogWarning(CityLanguageControllerLoggingEventID, "By-Sprog relation med CityId : "
        //                         + CityId.ToString() + " og languageId :  "
        //                         + LanguageId.ToString() + " ændret til : "
        //                         + CityLanguageDto_Object.CityId.ToString() + " og languageId : "
        //                         + CityLanguageDto_Object.LanguageId.ToString() + " ændret af : " + UserName);
        //#endif
        //                return Ok(CityLanguageToSave);
        //            }

        //            return NoContent();
        //        }

        //[HttpPut]
        //[Route("[action]")]
        //public async Task<IActionResult> UpdateCityLanguagesList([FromBody] List<CityLanguageForSaveAndUpdateDto> CityLanguageDto_Object_List,
        //                                                         bool DeleteOldElementsInListNotSpecifiedInCurrentList,
        //                                                         string UserName = "No Name")
        //{
        //    List<int> CurrentLanguageIds = new List<int>();

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (CityLanguageDto_Object_List.Count > 0)
        //    {
        //        int ListCounter;

        //        for (ListCounter = 1; ListCounter < CityLanguageDto_Object_List.Count; ListCounter++)
        //        {
        //            if (CityLanguageDto_Object_List[ListCounter].CityId != CityLanguageDto_Object_List[0].CityId)
        //            {
        //                return BadRequest("CityId givet i " + (ListCounter + 1).ToString() + ". element er forskellig fra det første CityID element i listen. De skal alle være ens !!!");
        //            }
        //        }

        //        var CityLanguageDto_Present_Object_List = await GetLanguagesForCity(CityLanguageDto_Object_List[0].CityId,
        //                                                                            false,
        //                                                                            UserName);

        //        var CityLanguageList = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetAllLanguagesFromCityID(CityLanguageDto_Object_List[0].CityId);

        //        //var Count = CityLanguageDto_Present_Object_List.Value;
        //        //if (CityLanguageDto_Present_Object_List is IList<CityLanguageDto>)

        //        if (true == DeleteOldElementsInListNotSpecifiedInCurrentList)
        //        {
        //            foreach (var CityLanguageCombination in CityLanguageList)
        //            {
        //                if (!_repositoryWrapper.CityLanguageRepositoryWrapper.LanguageIdFoundInCityLanguageList(CityLanguageDto_Object_List,
        //                                                                                                        CityLanguageCombination.LanguageId))
        //                {
        //                    await DeleteCityLanguage(CityLanguageDto_Object_List[0].CityId,
        //                                             CityLanguageCombination.LanguageId,
        //                                             UserName);
        //                }
        //            }
        //        }

        //        CityLanguageDto_Present_Object_List = await GetLanguagesForCity(CityLanguageDto_Object_List[0].CityId,
        //                                                                        false,
        //                                                                        UserName);

        //        CityLanguageList = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetAllLanguagesFromCityID(CityLanguageDto_Object_List[0].CityId);
        //        foreach (var CityLanguageCombination in CityLanguageList)
        //        {
        //            CurrentLanguageIds.Add(CityLanguageCombination.LanguageId);
        //        }

        //        for (ListCounter = 0; ListCounter < CityLanguageDto_Object_List.Count; ListCounter++)
        //        {
        //            if (!CurrentLanguageIds.Contains(CityLanguageDto_Object_List[ListCounter].LanguageId))
        //            {
        //                await CreateCityLanguage(CityLanguageDto_Object_List[ListCounter],
        //                                         UserName);
        //            }
        //        }
        //        return NoContent();
        //    }
        //    else
        //    {
        //        return BadRequest("Ingen CityLanguages specificeret !!!");
        //    }
        //}

        [HttpPut("UpdateCityLanguagesList")]
        public async Task<IActionResult> UpdateCityLanguagesList([FromBody] List<CityLanguageForSaveAndUpdateDto> CityLanguageForSaveAndUpdateDto_List,
                                                                 bool DeleteOldElementsInListsNotSpecifiedInCurrentLists = true,
                                                                 bool UseExtendedDatabaseDebugging = false,
                                                                 string UserName = "No Name")
        {
            try
            {
                ICommunicationResults CommunicationResults_Object;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                CommunicationResults_Object = await _cityLanguageService.UpdateCityLanguagesList(CityLanguageForSaveAndUpdateDto_List,
                                                                                                 DeleteOldElementsInListsNotSpecifiedInCurrentLists,
                                                                                                 UserName,
                                                                                                 UseExtendedDatabaseDebugging);

                if (CommunicationResults_Object.HasErrorOccured == true)
                {
                    _logger.LogError(CommunicationResults_Object.ResultString);
                }
                else
                {
                    _logger.LogInfo(CommunicationResults_Object.ResultString);
                }

                return StatusCode(CommunicationResults_Object.HttpStatusCodeResult, CommunicationResults_Object.ResultString);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside action UpdateCityLanguagesList for {UserName}: {Error.Message}");
                return StatusCode(500, "Internal server error for {UserName}");
            }
        }

        //        // DELETE api/<CityLanguageController>/5/4
        //        [HttpDelete("{CityId, LanguageId}")]
        //        public async Task<IActionResult> DeleteCityLanguage(int CityId, 
        //                                                            int LanguageId,
        //                                                            string UserName = "No Name")
        //        {
        //            _repositoryWrapper.CityLanguageRepositoryWrapper.DisableLazyLoading();

        //            var cityLanguageFromRepo = await _repositoryWrapper.CityLanguageRepositoryWrapper.GetCityIdLanguageIdCombination(CityId, LanguageId);

        //            if (null == cityLanguageFromRepo)
        //            {
        //                return NotFound();
        //            }

        //            await _repositoryWrapper.CityLanguageRepositoryWrapper.Delete(cityLanguageFromRepo);
        //#if Use_Hub_Logic_On_ServerSide
        //            await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
        //#endif

        //#if Test_Logging
        //            this._logger.LogWarning(CityLanguageControllerLoggingEventID, "By-Sprog relation med CityId : "
        //                     + CityId.ToString() + " og languageId :  "
        //                     + LanguageId.ToString() + 
        //                     " slettet af : " + UserName);
        //#endif

        //            return NoContent();
        //        }
    }
}
