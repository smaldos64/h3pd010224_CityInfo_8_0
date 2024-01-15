using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
//using h3pd040121_Projekt1_WebApi.ViewModels;

using Contracts;
using Entities.Models;
using Entities.DataTransferObjects;
using ServicesContracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo_8_0_Server.Controllers
{
    public class CityControllerParameters
    {
        // De 3 parametre herunder bestemmer hvordan output data fra controlleren her
        // præsenteres og behandles for klienten. 
        // Parameterne er medtaget først og fremmest af uddennelsesmæssige
        // formål, således at brugerne af controlleren her, kan se
        // hvordan man kan returnere (relationelle) data på forskellige måder fra en 
        // controller tilbage til en klient.

        public bool _use_Lazy_Loading_On_City_Controller = true;
        public bool _show_Cyclic_Data = false;
        public bool _use_Mapster = true;
    }

    //[Route("api/[controller]")]
    //[ApiController]

    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        private static CityControllerParameters _cityControllerParameters_Object = new CityControllerParameters();

        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;
        private ICityService _cityService;

#if Use_Hub_Logic_On_ServertSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif

        //        private readonly PointOfInterestController _pointOfInterestController;
        //        private readonly CityLanguageController _cityLanguageController;

        //        public CityController(IRepositoryWrapper repositoryWrapper,
        //                              PointOfInterestController pointOfInterestController,
        //                              CityLanguageController cityLanguageController

        //#if Use_Hub_Logic_On_ServertSide
        //                              , IHubContext<BroadcastHub> broadcastHub
        //#endif 
        //            )
        //        {
        //            this._repositoryWrapper = repositoryWrapper;
        //            this._pointOfInterestController = pointOfInterestController;
        //            this._cityLanguageController = cityLanguageController;

        //#if Use_Hub_Logic_On_ServertSide
        //            this._broadcastHub = broadcastHub;
        //#endif
        //        }

        public CityController(ILoggerManager logger, 
                              IRepositoryWrapper repository,
                              ICityService cityService )
        {
              this._logger = logger;
              this._repositoryWrapper = repository;
              this._cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCities(bool includeRelations = true,
                                                   bool UseLazyLoading = true,
                                                   bool UseMapster = true,
                                                   string UserName = "No Name")
        {
          try
          {
            IEnumerable<City> CityList = new List<City>();

            if ((false == includeRelations) || (false == UseLazyLoading))
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
              CityList = await _repositoryWrapper.CityRepositoryWrapper.GetAllCities(includeRelations);// as IEnumerable<City>; //as IQueryable<City>;
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
            _logger.LogInfo("All Cities has been read from GetCities action");
            return Ok(CityDtos);
          }
          catch (Exception Error)
          {
            _logger.LogError($"Something went wrong inside GetCities action: {Error.Message}");
            return StatusCode(500, "Internal server error");
          }
        }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetCitiesServiceLayer(bool includeRelations = true,
                                                           bool UseLazyLoading = true,
                                                           bool UseMapster = true,
                                                           string UserName = "No Name")
    {
      try
      {
        IEnumerable<City> CityList = new List<City>();

        if ((false == includeRelations) || (false == UseLazyLoading))
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
          CityList = await _cityService.GetCities(includeRelations);
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
        _logger.LogInfo("All Cities has been read from GetCities action");
        return Ok(CityDtos);
      }
      catch (Exception Error)
      {
        _logger.LogError($"Something went wrong inside GetCities action: {Error.Message}");
        return StatusCode(500, "Internal server error");
      }
    }

    //        [HttpGet("{CityId}", Name = "GetCity")]
    //        public async Task<IActionResult> GetCity(int CityId, 
    //                                                 bool includeRelations = true,
    //                                                 string UserName = "No Name")
    //        {
    //            if (false == includeRelations)
    //            {
    //                _repositoryWrapper.CityInfoRepositoryWrapper.DisableLazyLoading();
    //            }
    //            else
    //            {
    //                _repositoryWrapper.CityInfoRepositoryWrapper.EnableLazyLoading();
    //            }

    //            var City_Object = await _repositoryWrapper.CityInfoRepositoryWrapper.FindOne(CityId);

    //            if (null == City_Object)
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //#if Test_Logging
    //                Serilog.Context.LogContext.PushProperty("UserName", UserName); //Push user in LogContext;  
    //                this._logger.LogWarning(CityControllerLoggingEventID, "By med CityId : " + CityId.ToString() + " læst af : " + UserName);
    //#endif
    //                CityDto CityDto_Object = City_Object.Adapt<CityDto>();
    //                return Ok(CityDto_Object);
    //            }
    //        }

    //        // Metoden herunder er "kun" medtaget for test formål. Den bruges til at vise
    //        // hvordan data fra controlleren kan formatteres på forskellig måde.
    //        [HttpGet]
    //        [Route("[action]")]
    //        public async Task<IActionResult> GetCitiesDataTest(bool includeRelations = false)
    //        {
    //            if (_cityControllerParameters_Object._use_Lazy_Loading_On_City_Controller)
    //            {
    //                if (false == includeRelations)
    //                {
    //                    _repositoryWrapper.CityInfoRepositoryWrapper.DisableLazyLoading();

    //                    var cityEntities = await _repositoryWrapper.CityInfoRepositoryWrapper.FindAll();

    //                    var CityDtos = cityEntities.Adapt<CityDto[]>().ToList();
    //                    return Ok(CityDtos);
    //                }
    //                else  // true == includeRelations 
    //                {
    //                    _repositoryWrapper.CityInfoRepositoryWrapper.EnableLazyLoading();

    //                    var cityEntities = await _repositoryWrapper.CityInfoRepositoryWrapper.FindAll();

    //                    if (false == _cityControllerParameters_Object._use_Mapster)
    //                    {
    //                        // Her vises den kode, der i praksis udføres generisk ved brug af AutoMapper !!!
    //                        IEnumerable<CityDto> CityDtos = MapHere(cityEntities.ToList());
    //                        return Ok(CityDtos);
    //                    }
    //                    else
    //                    {
    //                        var CityDtos = cityEntities.Adapt<CityDto[]>().ToList();

    //                        if (_cityControllerParameters_Object._show_Cyclic_Data)
    //                        {
    //                            return Ok(cityEntities);
    //                        }
    //                        else
    //                        {
    //                            return Ok(CityDtos);
    //                        }
    //                    }
    //                }
    //            }
    //            else  // !_use_Lazy_Loading_On_City_Controller
    //            {
    //                _repositoryWrapper.CityInfoRepositoryWrapper.DisableLazyLoading();

    //                if (false == includeRelations)
    //                {
    //                    var cityEntities = await _repositoryWrapper.CityInfoRepositoryWrapper.GetAllCities(includeRelations);

    //                    IEnumerable<CityDto> CityDtos = MapHere(cityEntities.ToList());
    //                    return Ok(CityDtos);
    //                }
    //                else  // true == includeRelations 
    //                {
    //                    var cityEntities = await _repositoryWrapper.CityInfoRepositoryWrapper.GetAllCities(includeRelations);

    //                    IEnumerable<CityDto> CityDtos = MapHere(cityEntities.ToList());

    //                    //IEnumerable<CityDto> CityDtos = _mapper.Map<IEnumerable<CityDto>>(cityEntities);

    //                    if (_cityControllerParameters_Object._show_Cyclic_Data)
    //                    {
    //                        return Ok(cityEntities);
    //                    }
    //                    else
    //                    {
    //                        return Ok(CityDtos);
    //                    }
    //                }
    //            }
    //        }

    //        // POST: api/City
    //        [HttpPost]
    //        public async Task<IActionResult> CreateCity([FromBody] CityForSaveWithCountryDto CityDto_Object,
    //                                                    string UserName = "No Name")
    //        {
    //            if (CityDto_Object.Description == CityDto_Object.Name)
    //            {
    //                ModelState.AddModelError(
    //                    "Description",
    //                    "The provided description should be different from the name.");
    //            }

    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            City City_Object = CityDto_Object.Adapt<City>();
    //            await _repositoryWrapper.CityInfoRepositoryWrapper.Create(City_Object);

    //#if Use_Hub_Logic_On_ServertSide
    //            await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
    //#endif

    //#if Test_Logging
    //            //Serilog.Context.LogContext.PushProperty("UserName", UserName); //Push user in LogContext;  
    //            this._logger.LogWarning(CityControllerLoggingEventID, "By med CityId : " + City_Object.CityId.ToString() + 
    //                                    " og navn : " + City_Object.Name + " oprettet af : " + UserName);
    //#endif
    //            return Ok(City_Object.CityId);
    //        }

    //        // POST: api/City
    //        [HttpPost]
    //        [Route("[action]")]
    //        public async Task<IActionResult> CreateCityWithAllRelations([FromBody] SaveCityWithAllRelations SaveCityWithAllRelations_Object,
    //                                                                    string UserName = "No Name")
    //        {
    //            if (SaveCityWithAllRelations_Object.CityDto_Object.Description == SaveCityWithAllRelations_Object.CityDto_Object.Name)
    //            {
    //                ModelState.AddModelError(
    //                    "Description",
    //                    "The provided description should be different from the name.");
    //            }

    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            City City_Object = SaveCityWithAllRelations_Object.CityDto_Object.Adapt<City>();
    //            await _repositoryWrapper.CityInfoRepositoryWrapper.Create(City_Object);

    //            if (City_Object.CityId > 0)
    //            {
    //                if (null != SaveCityWithAllRelations_Object.PointOfInterests)
    //                {
    //                    for (int Counter = 0; Counter < SaveCityWithAllRelations_Object.PointOfInterests.Count; Counter++)
    //                    {
    //                        SaveCityWithAllRelations_Object.PointOfInterests[Counter].CityId = City_Object.CityId;
    //                        //PointOfInterestController PointOfInterestController_Object = new PointOfInterestController();
    //                        PointOfInterest PointOfInterest_Object = SaveCityWithAllRelations_Object.PointOfInterests[Counter].Adapt<PointOfInterest>();
    //                        await _repositoryWrapper.PointOfInterestRepositoryWrapper.Create(PointOfInterest_Object);
    //                        if (PointOfInterest_Object.PointOfInterestId <= 0)
    //                        {
    //                            return BadRequest("PointOfInterest kunne ikke gemmes");
    //                        }
    //                    }
    //                }

    //                if (null != SaveCityWithAllRelations_Object.CityLanguages)
    //                {
    //                    for (int Counter = 0; Counter < SaveCityWithAllRelations_Object.CityLanguages.Count; Counter++)
    //                    {
    //                        SaveCityWithAllRelations_Object.CityLanguages[Counter].CityId = City_Object.CityId;

    //                        CityLanguage CityLanguage_Object = new CityLanguage();

    //                        if (CityLanguage_Object.CloneData<CityLanguage>(SaveCityWithAllRelations_Object.CityLanguages[Counter]))
    //                        {
    //                            await _repositoryWrapper.CityLanguageRepositoryWrapper.Create(CityLanguage_Object);
    //                        }
    //                        else
    //                        {
    //                            return BadRequest("CityLanguage Object kunne ikke genereres");
    //                        }
    //                    }
    //                }
    //            }

    //#if Use_Hub_Logic_On_ServertSide
    //            await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
    //#endif

    //#if Test_Logging
    //            //Serilog.Context.LogContext.PushProperty("UserName", UserName); //Push user in LogContext;  
    //            this._logger.LogWarning(CityControllerLoggingEventID, "By med CityId : " + City_Object.CityId.ToString() +
    //                                    " og navn : " + City_Object.Name + 
    //                                    "og relationer oprettet af : " + UserName);
    //#endif
    //            return Ok(City_Object.CityId);
    //        }


    //        // POST: api/Controller Parameters
    //        [HttpPost]
    //        [Route("[action]")]
    //        public IActionResult PostControllerSettingParameters(bool Use_Lazy_Loading_On_City_Controller = true,
    //                                                             bool Show_Cyclic_Data = false,
    //                                                             bool Use_Mapster = true)
    //        {
    //            _cityControllerParameters_Object._use_Lazy_Loading_On_City_Controller = Use_Lazy_Loading_On_City_Controller;
    //            _cityControllerParameters_Object._show_Cyclic_Data = Show_Cyclic_Data;
    //            _cityControllerParameters_Object._use_Mapster = Use_Mapster;

    //            return Ok(_cityControllerParameters_Object);
    //        }

    //        // PUT: api/City/5
    //        [HttpPut("{CityId}")]
    //        public async Task<IActionResult> UpdateCity(int CityId, 
    //                                                    [FromBody] CityForUpdateDto CityDto_Object,
    //                                                    string UserName = "No Name")
    //        {
    //            if (CityId != CityDto_Object.CityId)
    //            {
    //                return BadRequest();
    //            }

    //            if (CityDto_Object.Description == CityDto_Object.Name)
    //            {
    //                ModelState.AddModelError(
    //                    "Description",
    //                    "The provided description should be different from the name.");
    //            }

    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            var cityFromRepo = await _repositoryWrapper.CityInfoRepositoryWrapper.FindOne(CityId);

    //            if (null == cityFromRepo)
    //            {
    //                return NotFound();
    //            }

    //            // Dur ikke med en Mapster Adapt i tilfældet med en update !!!
    //            // Derfor har jeg lavet min egen statiske metode CloneData til at kopiere 
    //            // data mellem 2 (generiske) objeter. Denne meode er lavet som en statisk metode i
    //            // en statisk klasse og kan derfor kaldes som en extension metode.
    //            // Metoden kan findes i filen Extensions/MyMapster.cs

    //            //var cityFromRepo1 = CityDto_Object.Adapt<City>();

    //            if (cityFromRepo.CloneData<City>(CityDto_Object))
    //            {
    //                await _repositoryWrapper.CityInfoRepositoryWrapper.Update(cityFromRepo);
    //#if Use_Hub_Logic_On_ServertSide
    //                await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
    //#endif

    //#if Test_Logging
    //                //Serilog.Context.LogContext.PushProperty("UserName", UserName); //Push user in LogContext;  
    //                this._logger.LogWarning(CityControllerLoggingEventID, "By med CityId : " + CityDto_Object.CityId.ToString() + " ændret til (Name) : " +
    //                                        CityDto_Object.Name + " (Description) : " +
    //                                        CityDto_Object.Description + " (CountryId) : " +
    //                                        CityDto_Object.CountryID + " ændret af : " + UserName);
    //#endif
    //            }
    //            //_repositoryWrapper.CityInfoRepositoryWrapper.Save();

    //            return NoContent();
    //        }

    //        //[HttpPut("{CityId}/updateall")]
    //        [HttpPut("{CityId}/{DeleteOldElementsInListsNotSpecifiedInCurrentLists}")]

    //        public async Task<IActionResult> UpdateCityWithAllRelations(int CityId,
    //                                                                    //bool DeleteOldElementsInListsNotSpecifiedInCurrentLists = true,
    //                                                                    [FromBody] UpdateCityWithAllRelations UpdateCityWithAllRelations_Object,
    //                                                                    bool DeleteOldElementsInListsNotSpecifiedInCurrentLists = true,
    //                                                                    string UserName = "No Name")
    //        {
    //            List<int> AddedList = new List<int>();
    //            int ListCounter = 0;

    //            if (CityId != UpdateCityWithAllRelations_Object.CityDto_Object.CityId)
    //            {
    //                return BadRequest();
    //            }

    //            if (UpdateCityWithAllRelations_Object.CityDto_Object.Description ==
    //                UpdateCityWithAllRelations_Object.CityDto_Object.Name)
    //            {
    //                ModelState.AddModelError(
    //                    "Description",
    //                    "The provided description should be different from the name.");
    //            }

    //            if (!ModelState.IsValid)
    //            {
    //                return BadRequest(ModelState);
    //            }

    //            var cityFromRepo = await _repositoryWrapper.CityInfoRepositoryWrapper.FindOne(CityId);

    //            if (null == cityFromRepo)
    //            {
    //                return NotFound();
    //            }

    //            // Dur ikke med en Mapster Adapt i tilfældet med en update !!!
    //            // Derfor har jeg lavet min egen statiske metode CloneData til at kopiere 
    //            // data mellem 2 (generiske) objeter. Denne metode er lavet som en statisk metode i
    //            // en statisk klasse og kan derfor kaldes som en extension metode.
    //            // Metoden kan findes i filen Extensions/MyMapster.cs

    //            if (cityFromRepo.CloneData<City>(UpdateCityWithAllRelations_Object.CityDto_Object))
    //            {
    //                await _repositoryWrapper.CityInfoRepositoryWrapper.Update(cityFromRepo);
    //#if Use_Hub_Logic_On_ServertSide
    //                await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
    //#endif

    //#if Test_Logging
    //                //Serilog.Context.LogContext.PushProperty("UserName", UserName); //Push user in LogContext;  
    //                this._logger.LogWarning(CityControllerLoggingEventID, "By med CityId : " + UpdateCityWithAllRelations_Object.CityDto_Object.CityId.ToString() + " ændret til (Name) : " +
    //                                        UpdateCityWithAllRelations_Object.CityDto_Object.Name + " (Description) : " +
    //                                        UpdateCityWithAllRelations_Object.CityDto_Object.Description + " (CountryId) : " +
    //                                        UpdateCityWithAllRelations_Object.CityDto_Object.CountryID + " ændret af : " + UserName);
    //#endif
    //            }
    //            else
    //            {
    //                return BadRequest("Data Clone Fejl !!!");
    //            }

    //            if (null != UpdateCityWithAllRelations_Object.PointOfInterests)
    //            {
    //                for (int Counter = 0; Counter < UpdateCityWithAllRelations_Object.PointOfInterests.Count; Counter++)
    //                {
    //                    if (0 == UpdateCityWithAllRelations_Object.PointOfInterests[Counter].PointOfInterestId)
    //                    {
    //                        PointOfInterestForSaveWithCityDto PointOfInterestForSaveWithCityDto_Object =
    //                            UpdateCityWithAllRelations_Object.PointOfInterests[Counter].Adapt<PointOfInterestForSaveWithCityDto>();

    //                        var ActionResultAdd = await _pointOfInterestController.AddPointOfInterest(PointOfInterestForSaveWithCityDto_Object,
    //                                                                                                  UserName);
    //                        var OkResultActionResultAdd = ActionResultAdd as OkObjectResult;

    //                        if (null != OkResultActionResultAdd)
    //                        {
    //                            int Test = (int)OkResultActionResultAdd.Value;
    //                            AddedList.Add((int)OkResultActionResultAdd.Value);
    //                        }
    //                        else
    //                        {
    //                            string ErrorString = "Add Fejl i PointOfInterest objekt nummer " + Counter.ToString() + " !!!";
    //                            return BadRequest(ErrorString);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        var ActionResultUpdate = await _pointOfInterestController.UpdatePointOfInterest(UpdateCityWithAllRelations_Object.PointOfInterests[Counter].PointOfInterestId,
    //                                                                                                        UpdateCityWithAllRelations_Object.PointOfInterests[Counter],
    //                                                                                                        UserName);
    //                        var NoContentResultActionResultUpdate = ActionResultUpdate as NoContentResult;

    //                        if (null == NoContentResultActionResultUpdate) 
    //                        {
    //                            string ErrorString = "Update Fejl i PointOfInterest objekt nummer " + Counter.ToString() + " !!!";
    //                            return BadRequest(ErrorString);
    //                        }
    //                    }
    //                }

    //                if (true == DeleteOldElementsInListsNotSpecifiedInCurrentLists)
    //                {
    //                    var PointOfInterestList = await _repositoryWrapper.PointOfInterestRepositoryWrapper.GetAllPointOfInterestWithCityID(CityId, false);
    //                    ListCounter = 1;

    //                    foreach (PointOfInterest PointOfInterest_object in PointOfInterestList)
    //                    {
    //                        var Matches = UpdateCityWithAllRelations_Object.PointOfInterests.Where(p => p.PointOfInterestId == PointOfInterest_object.PointOfInterestId);
    //                        if (0 == Matches.Count())
    //                        {
    //                            var Matches1 = AddedList.Any(p => p == PointOfInterest_object.PointOfInterestId);

    //                            if (!Matches1)
    //                            {
    //                                // Et af de nuværende PointOfinterests for det angivne CityId
    //                                // findes ikke i den nye liste over ønskede opdateringer og heller
    //                                // ikke i liste for nye PointOfInterests for det angivne CityId. 
    //                                // Og desuden er parameteren for at slette "gamle" elementer i
    //                                // PointOfInterest listen for det angivne CityId sat. Så slet 
    //                                // dette PointOfInterest fra databasen !!!
    //                                var ActionResultDelete = await _pointOfInterestController.DeletePointOfInterest(PointOfInterest_object.PointOfInterestId,
    //                                                                                                                UserName);
    //                                var NoContentActionResultDelete = ActionResultDelete as NoContentResult;

    //                                if (null == NoContentActionResultDelete)
    //                                {
    //                                    string ErrorString = "Delete Fejl i PointOfInterest objekt nummer " + ListCounter.ToString() + " med PointOfInterestId " +
    //                                                          PointOfInterest_object.PointOfInterestId.ToString() + " i Databasen !!!";
    //                                    return BadRequest(ErrorString);
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            if (null != UpdateCityWithAllRelations_Object.CityLanguages)
    //            {
    //                var ActionResultUpdateCityLanguageList = await this._cityLanguageController.UpdateCityLanguagesList(UpdateCityWithAllRelations_Object.CityLanguages,
    //                                                                                                                    DeleteOldElementsInListsNotSpecifiedInCurrentLists,
    //                                                                                                                    UserName);
    //                var NoContentActionResultUpdateCityLanguageList = ActionResultUpdateCityLanguageList as NoContentResult;

    //                if (null == NoContentActionResultUpdateCityLanguageList)
    //                {
    //                    var BadRequestActionResultUpdateCityLanguageList = ActionResultUpdateCityLanguageList as BadRequestObjectResult;

    //                    string ErrorString = (string)(BadRequestActionResultUpdateCityLanguageList.Value);

    //                    return BadRequest(ErrorString);
    //                }

    //            }

    //            return NoContent();
    //        }

    //        // DELETE: api/5
    //        [HttpDelete("{CityId}")]
    //        public async Task<IActionResult> DeleteCity(int CityId,
    //                                                    string UserName = "No Name")
    //        {
    //            _repositoryWrapper.CityInfoRepositoryWrapper.DisableLazyLoading();

    //            var cityFromRepo = await _repositoryWrapper.CityInfoRepositoryWrapper.FindOne(CityId);

    //            if (null == cityFromRepo)
    //            {
    //                return NotFound();
    //            }

    //            await _repositoryWrapper.CityInfoRepositoryWrapper.Delete(cityFromRepo);
    //#if Use_Hub_Logic_On_ServertSide
    //            await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
    //#endif

    //#if Test_Logging
    //            //Serilog.Context.LogContext.PushProperty("UserName", UserName); //Push user in LogContext;  
    //            this._logger.LogWarning(CityControllerLoggingEventID, "By med CityId : " + CityId.ToString() + " slettet af : " + UserName);
    //#endif

    //            return NoContent();
    //        }

    private List<CityDto> MapHere(List<City> Cities)
        {
            List<CityDto> CityDtos = new List<CityDto>();
            //ICollection<CityDto> CityDtosI = new List<CityDto>();

            for (int Counter = 0; Counter < Cities.Count(); Counter++)
            {
                CityDto CityDto_Object = new CityDto();

                CityDto_Object.CityId = Cities[Counter].CityId;
                CityDto_Object.CityName = Cities[Counter].CityName;
                CityDto_Object.CityDescription = Cities[Counter].CityDescription;
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

                CityDto_Object.CityLanguages = new List<LanguageDtoMinusRelations>();
                for (int CityLanguageCounter = 0;
                    CityLanguageCounter < Cities[Counter].CityLanguages.Count();
                    CityLanguageCounter++)
                {
                    LanguageDtoMinusRelations LanguageDto_Object = new LanguageDtoMinusRelations();
                    LanguageDto_Object.LanguageId = Cities[Counter].CityLanguages.ElementAt(CityLanguageCounter).LanguageId;
                    LanguageDto_Object.LanguageName = Cities[Counter].CityLanguages.ElementAt(CityLanguageCounter).Language.LanguageName;

                    CityDto_Object.CityLanguages.Add(LanguageDto_Object);
                }
                CityDtos.Add(CityDto_Object);
            }

            CityDto CityDto_Object_Final = new CityDto();
            CityDto_Object_Final.CityId = 0;
            CityDto_Object_Final.CityName = "Egen Konvertering !!!";
            CityDto_Object_Final.CityDescription = "Det sidste objekt her er lavet for at illustrere det arbejde, som AutoMapper gør for os !!!";

            CityDtos.Add(CityDto_Object_Final);

            return (CityDtos);
        }
    }
}
