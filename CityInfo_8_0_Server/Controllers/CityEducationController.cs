using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using Contracts;
using Entities.Models;
using Entities.DataTransferObjects;
using ServicesContracts;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using CityInfo_8_0_Server.ViewModels;
using CityInfo_8_0_Server.Extensions;
using Entities.MyMapsterFunctions;

#if Use_Hub_Logic_On_ServerSide
using CityInfo_8_0_Server.Hubs;
#endif

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// Formålet med denne Controller er at vise noget udvidet kode i forhold til 
// CityController.cs controller filen. Den udvidede kode er først og fremmest
// medtaget for læringens skyld. Dette forstået på den måde at man som elev kan gå
// ind i controlleren her og se forskellige måder at gøre ting på. 
// Deraf navnet på filen => CityEducationController.cs

namespace CityInfo_8_0_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityEducationController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;
        private ICityService _cityService;

#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif
        public CityEducationController(ILoggerManager logger,
                              IRepositoryWrapper repositoryWrapper,
                              ICityService cityService)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
            this._cityService = cityService;
        }

        [HttpGet("GetCities")]
        public async Task<IActionResult> GetCities(bool includeRelations = true,
                                                   bool UseLazyLoading = true,
                                                   bool UseMapster = true,
                                                   string UserName = "No Name")
        {
            try
            {
                IEnumerable<City> CityList = new List<City>();

                if (false == UseLazyLoading)
                {
                    _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();
                }
                else
                {
                    _repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();
                }

                if (true == UseLazyLoading)
                {
                    CityList = await _repositoryWrapper.CityRepositoryWrapper.FindAll();
                }
                else
                {
                    CityList = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(includeRelations);
                }

                // Koden der er udkommenteret herunder er med for at vise, at man kan nå alle
                // wrappere fra alle controllers. 
                //var LanguageEntities = _repositoryWrapper.LanguageRepositoryWrapper.FindAll();

                List<CityDto> CityDtos;

                if (true == UseMapster)
                {
                    CityDtos = CityList.Adapt<CityDto[]>().ToList();
                }
                else
                {
                    CityDtos = MapHere(CityList.ToList());
               }
                _logger.LogInfo($"All Cities has been read from GetCities action by {UserName}");
                return Ok(CityDtos);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCities action for {UserName} : {Error.Message}");
                return StatusCode(500, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetCitiesServiceLayer")]
        public async Task<IActionResult> GetCitiesServiceLayer(bool includeRelations = true,
                                                               bool UseLazyLoading = true,
                                                               bool UseMapster = true,
                                                               string UserName = "No Name")
        {
            try
            {
                IEnumerable<City> CityList = new List<City>();

                if (false == UseLazyLoading)
                {
                    _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();
                }
                else  // true == includeRelations && true == UseLazyLoading 
                {
                    _repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();
                }

                if (true == UseLazyLoading)
                {
                    CityList = await _repositoryWrapper.CityRepositoryWrapper.FindAll();
                }
                else
                {
                    CityList = await _cityService.GetAllCities(includeRelations);
                }

                List<CityDto> CityDtos;

                if (true == UseMapster)
                {
                    CityDtos = CityList.Adapt<CityDto[]>().ToList();
                }
                else
                {
                    CityDtos = MapHere(CityList.ToList());
                }
                _logger.LogInfo($"All Cities has been read from GetCitiesServiceLayer action by {UserName}");
                return Ok(CityDtos);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCitiesServiceLayer action for {UserName}: {Error.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetSpecifiedNumberOfCities")]
        public async Task<IActionResult> GetSpecifiedNumberOfCities(bool includeRelations = true,
                                                                    bool UseQueryable = true,
                                                                    int NumberOfCities = 5,
                                                                    string UserName = "No Name")
        {
            try
            {
                IEnumerable<City> CityList = new List<City>();

                _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();

                CityList = await _repositoryWrapper.CityRepositoryWrapper.GetSpecifiedNumberOfCities(NumberOfCities, includeRelations, UseQueryable);

                List<CityDto> CityDtos;

                CityDtos = CityList.Adapt<CityDto[]>().ToList();

                _logger.LogInfo($"{NumberOfCities} Cities has been read from GetCities action by {UserName}");
                return Ok(CityDtos);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCities action for {UserName}: {Error.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        //[HttpGet("{CityId}", Name = "GetCity")]
        [HttpGet("GetCity/{CityId}")]
        public async Task<IActionResult> GetCity(int CityId,
                                                 bool UseLazyLoading = true,
                                                 string UserName = "No Name")
        {
            if (false == UseLazyLoading)
            {
                _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();
            }
            else
            {
                _repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();
            }

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
        [HttpPost("CreateCity")]
        public async Task<IActionResult> CreateCity([FromBody] CityForSaveDto CityDto_Object,
                                                    string UserName = "No Name")
        {
            try
            {
                //HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError; 
                //httpStatusCode = HttpStatusCode.OK;
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

        [HttpPost("CreateCityWithAllRelations")]
        public async Task<IActionResult> CreateCityWithAllRelations([FromBody] SaveCityWithAllRelations SaveCityWithAllRelations_Object,
                                                                    bool UseExtendedDatabaseDebugging = false,
                                                                    string UserName = "No Name")
        {
            try
            {
                ICommunicationResults CommunicationResults_Object;

                if (SaveCityWithAllRelations_Object.CityDto_Object.CityDescription ==
                    SaveCityWithAllRelations_Object.CityDto_Object.CityName)
                {
                    ModelState.AddModelError(
                        "Description",
                        "The provided description should be different from the name.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"ModelState is Invalid for {UserName} in action CreateCityWithAllRelations");
                    return BadRequest(ModelState);
                }

                CommunicationResults_Object = await _cityService.SaveCityWithAllInfo(SaveCityWithAllRelations_Object.CityDto_Object,
                                                                                     SaveCityWithAllRelations_Object.PointOfInterests,
                                                                                     SaveCityWithAllRelations_Object.CityLanguages,
                                                                                     UserName,
                                                                                     UseExtendedDatabaseDebugging);

                if (CommunicationResults_Object.HasErrorOccured == true)
                {
                    _logger.LogError(CommunicationResults_Object.ResultString);
                }
                else
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
                    _logger.LogInfo(CommunicationResults_Object.ResultString);
                }

                return StatusCode(CommunicationResults_Object.HttpStatusCodeResult, CommunicationResults_Object.ResultString);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside action CreateCityWithAllRelations for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error for {UserName}");
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

        [HttpPut("UpdateCityWithAllRelations/{CityId}")]
        public async Task<IActionResult> UpdateCityWithAllRelations(int CityId,
                                                                    [FromBody] UpdateCityWithAllRelations UpdateCityWithAllRelations_Object,
                                                                    bool DeleteOldElementsInListsNotSpecifiedInCurrentLists = true,
                                                                    bool UseExtendedDatabaseDebugging = false,
                                                                    string UserName = "No Name")
        {
            try
            {
                ICommunicationResults CommunicationResults_Object;

                if (CityId != UpdateCityWithAllRelations_Object.CityDto_Object.CityId)
                {
                    return BadRequest("CityID error !!!");
                }

                if (UpdateCityWithAllRelations_Object.CityDto_Object.CityDescription ==
                    UpdateCityWithAllRelations_Object.CityDto_Object.CityName)
                {
                    ModelState.AddModelError(
                        "Description",
                        "The provided description should be different from the name.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"ModelState is Invalid for {UserName} in action UpdateCityWithAllRelations");
                    return BadRequest(ModelState);
                }

                CommunicationResults_Object = await _cityService.UpdateCityWithAllRelations(UpdateCityWithAllRelations_Object.CityDto_Object,
                                                                                            UpdateCityWithAllRelations_Object.PointOfInterests,
                                                                                            UpdateCityWithAllRelations_Object.CityLanguages,
                                                                                            UserName,
                                                                                            DeleteOldElementsInListsNotSpecifiedInCurrentLists,
                                                                                            UseExtendedDatabaseDebugging);

                if (CommunicationResults_Object.HasErrorOccured == true)
                {
                    _logger.LogError(CommunicationResults_Object.ResultString);
                }
                else
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
                    _logger.LogInfo(CommunicationResults_Object.ResultString);
                }

                return StatusCode(CommunicationResults_Object.HttpStatusCodeResult, CommunicationResults_Object.ResultString);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside action UpdateCityWithAllRelations for {UserName}: {Error.Message}");
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

        private List<CityDto> MapHere(List<City> Cities)
        {
            List<CityDto> CityDtos = new List<CityDto>();

            for (int Counter = 0; Counter < Cities.Count(); Counter++)
            {
                CityDto CityDto_Object = new CityDto();

                CityDto_Object.CityId = Cities[Counter].CityId;
                CityDto_Object.CityName = Cities[Counter].CityName;
                CityDto_Object.CityDescription = Cities[Counter].CityDescription;
                CityDto_Object.CountryID = Cities[Counter].CountryID;
                CityDto_Object.PointsOfInterest = new List<PointOfInterestForUpdateDto>();

                for (int PointOfInterestsCounter = 0;
                    PointOfInterestsCounter < Cities[Counter].PointsOfInterest.Count();
                    PointOfInterestsCounter++)
                {
                    PointOfInterestForUpdateDto PointOfInterestDto_Object = new PointOfInterestForUpdateDto();
                    PointOfInterestDto_Object.PointOfInterestId = Cities[Counter].PointsOfInterest.ElementAt(PointOfInterestsCounter).PointOfInterestId;
                    PointOfInterestDto_Object.CityId = Cities[Counter].PointsOfInterest.ElementAt(PointOfInterestsCounter).CityId;
                    PointOfInterestDto_Object.PointOfInterestName = Cities[Counter].PointsOfInterest.ElementAt(PointOfInterestsCounter).PointOfInterestName;
                    PointOfInterestDto_Object.PointOfInterestDescription = Cities[Counter].PointsOfInterest.ElementAt(PointOfInterestsCounter).PointOfInterestDescription;

                    CityDto_Object.PointsOfInterest.Add(PointOfInterestDto_Object);
                }

                CityDto_Object.CityLanguages = new List<CityLanguageDtoMinusCityRelations>();
                for (int CityLanguageCounter = 0;
                    CityLanguageCounter < Cities[Counter].CityLanguages.Count();
                    CityLanguageCounter++)
                {
                    CityLanguageDtoMinusCityRelations LanguageDto_Object = new CityLanguageDtoMinusCityRelations();
                    LanguageDto_Object.Language = new LanguageDtoMinusRelations();
                    LanguageDto_Object.Language.LanguageId = Cities[Counter].CityLanguages.ElementAt(CityLanguageCounter).LanguageId;
                    LanguageDto_Object.Language.LanguageName = Cities[Counter].CityLanguages.ElementAt(CityLanguageCounter).Language.LanguageName;

                    CityDto_Object.CityLanguages.Add(LanguageDto_Object);
                }
                CityDtos.Add(CityDto_Object);
            }

            //CityDto CityDto_Object_Final = new CityDto();
            //CityDto_Object_Final.CityId = 0;
            //CityDto_Object_Final.CityName = "Egen Konvertering !!!";
            //CityDto_Object_Final.CityDescription = "Det sidste objekt her er lavet for at illustrere det arbejde, som AutoMapper gør for os !!!";

            //CityDtos.Add(CityDto_Object_Final);

            return (CityDtos);
        }
    }
}
