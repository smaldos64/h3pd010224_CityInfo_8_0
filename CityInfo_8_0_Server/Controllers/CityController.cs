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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            _logger.LogInfo($"All Cities has been read from GetCities action by {UserName}");
            return Ok(CityDtos);
          }
          catch (Exception Error)
          {
            _logger.LogError($"Something went wrong inside GetCities action for {UserName} : {Error.Message}");
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
        _logger.LogInfo($"All Cities has been read from GetCitiesServiceLayer action by {UserName}");
        return Ok(CityDtos);
      }
      catch (Exception Error)
      {
        _logger.LogError($"Something went wrong inside GetCitiesServiceLayer action for {UserName}: {Error.Message}");
        return StatusCode(500, "Internal server error");
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
          return BadRequest(ModelState);
        }

        City City_Object = CityDto_Object.Adapt<City>();

        await _repositoryWrapper.CityRepositoryWrapper.Create(City_Object);
        NumberOfObjectsSaved = await _repositoryWrapper.CityRepositoryWrapper.Save();

#if Use_Hub_Logic_On_ServertSide
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
        return StatusCode(500, $"Internal server error for {UserName}");
      }
    }

    // POST: api/City
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> CreateCityServiceLayer([FromBody] CityForSaveWithCountryDto CityDto_Object,
                                                             string UserName = "No Name")
    {
      try
      {
        int NumberOfObjectsSavedThroughServiceLayer = 0;
        if (CityDto_Object.CityDescription == CityDto_Object.CityName)
        {
          ModelState.AddModelError(
              "Description",
              "The provided description should be different from the name.");
        }

        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }

        City City_Object = CityDto_Object.Adapt<City>();

        NumberOfObjectsSavedThroughServiceLayer = await _cityService.SaveCity(City_Object);
        
#if Use_Hub_Logic_On_ServertSide
        await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
        if (1 == NumberOfObjectsSavedThroughServiceLayer)
        {
          _logger.LogInfo($"City with Id : {City_Object.CityId} has been stored using Service Layer by {UserName}!!!");
          return Ok(City_Object.CityId);
        }
        else
        {
          _logger.LogError($"Error when saving City using Service Layer by {UserName} !!!");
          return BadRequest($"Error when saving City using Service Layer by {UserName} !!!");
        }
      }
      catch (Exception Error)
      {
        _logger.LogError($"Something went wrong inside Save City action for {UserName}: {Error.Message}");
        return StatusCode(500, "Internal server error for {UserName}");
      }
    }
    
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
