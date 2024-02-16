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

#if Use_Hub_Logic_On_ServertSide
using CityInfo_8_0_Server.Hubs;
#endif

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo_8_0_Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Route("[controller]")]
    public class CityController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;
        private ICityService _cityService;

#if Use_Hub_Logic_On_ServertSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif
        public CityController(ILoggerManager logger, 
                              IRepositoryWrapper repositoryWrapper,
                              ICityService cityService )
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

    [HttpGet("GetCitiesServiceLayer")]
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

    [HttpGet("GetSpecifiedNumberOfCities")]
    public async Task<IActionResult> GetSpecifiedNumberOfCities(bool includeRelations = true,
                                                                bool UseLazyLoading = true,
                                                                bool UseMapster = true,
                                                                bool UseQueryable = true,
                                                                int NumberOfCities = 5,
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

        CityList = await _repositoryWrapper.CityRepositoryWrapper.GetSpecifiedNumberOfCities(NumberOfCities, includeRelations, UseQueryable);
        
        List<CityDto> CityDtos;

        if (true == UseMapster)
        {
          CityDtos = CityList.Adapt<CityDto[]>().ToList();
        }
        else
        {
          CityDtos = MapHere(CityList.ToList());
        }
        _logger.LogInfo($"{NumberOfCities} Cities has been read from GetCities action by {UserName}");
        return Ok(CityDtos);
      }
      catch (Exception Error)
      {
        _logger.LogError($"Something went wrong inside GetCities action for {UserName}: {Error.Message}");
        return StatusCode(500, "Internal server error");
      }
    }

    // POST: api/City
    [HttpPost("CreateCity")]
    //[HttpPost]
    public async Task<IActionResult> CreateCity([FromBody] CityForSaveWithCountryDto CityDto_Object,
                                                string UserName = "No Name")
    {
      //string UserName = "No Name";
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
        NumberOfObjectsSaved = _repositoryWrapper.Save();

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

//    // POST: api/City
//    [HttpPost("CreateCityServiceLayer")]
//    //[Route("[action]")]
//    public async Task<IActionResult> CreateCityServiceLayer([FromBody] CityForSaveWithCountryDto CityForSaveWithCountryDto_Object,
//                                                             string UserName = "No Name")
//    {
//      try
//      {
//        int NumberOfObjectsSavedThroughServiceLayer = 0;
//        if (CityForSaveWithCountryDto_Object.CityDescription == CityForSaveWithCountryDto_Object.CityName)
//        {
//          ModelState.AddModelError(
//              "Description",
//              "The provided description should be different from the name.");
//        }

//        if (!ModelState.IsValid)
//        {
//          _logger.LogError($"ModelState is Invalid for {UserName} in action CreateCityServiceLayer");
//          return BadRequest(ModelState);
//        }

//        City City_Object = CityForSaveWithCountryDto_Object.Adapt<City>();

//        NumberOfObjectsSavedThroughServiceLayer = await _cityService.SaveCity(City_Object);
        
//#if Use_Hub_Logic_On_ServertSide
//        await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
//#endif
//        if (1 == NumberOfObjectsSavedThroughServiceLayer)
//        {
//          _logger.LogInfo($"City with Id : {City_Object.CityId} has been stored using Service Layer by {UserName}!!!");
//          return Ok(City_Object.CityId);
//        }
//        else
//        {
//          _logger.LogError($"Error when saving City using Service Layer by {UserName} !!!");
//          return BadRequest($"Error when saving City using Service Layer by {UserName} !!!");
//        }
//      }
//      catch (Exception Error)
//      {
//        _logger.LogError($"Something went wrong inside Save City action for {UserName}: {Error.Message}");
//        return StatusCode(500, "Internal server error for {UserName}");
//      }
//    }

//    // POST: api/City
//    [HttpPost("CreateCityWithAllRelations")]
//    public async Task<IActionResult> CreateCityWithAllRelations([FromBody] SaveCityWithAllRelations SaveCityWithAllRelations_Object,
//                                                                string UserName = "No Name")
//    {
//      int NumberOfObjectsSaved = 0;
//      try
//      {
//        if (SaveCityWithAllRelations_Object.CityDto_Object.CityDescription == SaveCityWithAllRelations_Object.CityDto_Object.CityName)
//        {
//          ModelState.AddModelError(
//              "Description",
//              "The provided description should be different from the name.");
//        }

//        if (!ModelState.IsValid)
//        {
//          _logger.LogError($"ModelState is Invalid for {UserName} in action CreateCityWithAllRelations");
//          return BadRequest(ModelState);
//        }

//        City City_Object = SaveCityWithAllRelations_Object.CityDto_Object.Adapt<City>();
//        await _repositoryWrapper.CityRepositoryWrapper.Create(City_Object);
//        NumberOfObjectsSaved = await _repositoryWrapper.CityRepositoryWrapper.Save();

//        if (1 == NumberOfObjectsSaved)
//        {
//          if (null != SaveCityWithAllRelations_Object.PointOfInterests)
//          {
//            for (int Counter = 0; Counter < SaveCityWithAllRelations_Object.PointOfInterests.Count; Counter++)
//            {
//              SaveCityWithAllRelations_Object.PointOfInterests[Counter].CityId = City_Object.CityId;
//              PointOfInterest PointOfInterest_Object = SaveCityWithAllRelations_Object.PointOfInterests[Counter].Adapt<PointOfInterest>();
//              await _repositoryWrapper.PointOfInterestRepositoryWrapper.Create(PointOfInterest_Object);
//              //if (PointOfInterest_Object.PointOfInterestId <= 0)
//              //{
//              //  return BadRequest("PointOfInterest kunne ikke gemmes");
//              //}
//            }
//          }

//          if (null != SaveCityWithAllRelations_Object.CityLanguages)
//          {
//            for (int Counter = 0; Counter < SaveCityWithAllRelations_Object.CityLanguages.Count; Counter++)
//            {
//              SaveCityWithAllRelations_Object.CityLanguages[Counter].CityId = City_Object.CityId;

//              CityLanguage CityLanguage_Object = new CityLanguage();

//              if (CityLanguage_Object.CloneData<CityLanguage>(SaveCityWithAllRelations_Object.CityLanguages[Counter]))
//              {
//                await _repositoryWrapper.CityLanguageRepositoryWrapper.Create(CityLanguage_Object);
//              }
//              else
//              {
//                return BadRequest("CityLanguage Object kunne ikke genereres");
//              }
//            }
//          }

//#if Use_Hub_Logic_On_ServertSide
//          await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
//#endif

//          NumberOfObjectsSaved = await _repositoryWrapper.CityRepositoryWrapper.Save();

//          int NumberOfObjectsSavedBesideCity = SaveCityWithAllRelations_Object.CityLanguages.Count +
//            SaveCityWithAllRelations_Object.PointOfInterests.Count;

//          if (NumberOfObjectsSaved == SaveCityWithAllRelations_Object.CityLanguages.Count +
//            SaveCityWithAllRelations_Object.PointOfInterests.Count)
//          {
//            _logger.LogInfo($"City with Id : {City_Object.CityId} has been saved together with {NumberOfObjectsSavedBesideCity} CityLanguages + PointOfInterests by {UserName} !!!");
//            return Ok(City_Object.CityId);
//          }
//          else
//          {
//            _logger.LogError($"Error when saving CityLanguages or PointOfInterests for CityId : {City_Object.CityId} by {UserName} !!!");
//            return BadRequest($"Error when saving CityLanguages or PointOfInterests for CityId : {City_Object.CityId} by {UserName} !!!");
//          }
//        }
//        else
//        {
//          _logger.LogError($"Something went wrong when saving City for {UserName}");
//          return BadRequest($"Something went wrong when saving City for {UserName}");
//        }
//      }
//      catch (Exception Error)
//      {
//        _logger.LogError($"Something went wrong inside Save City action for {UserName}: {Error.Message}");
//        return StatusCode(500, "Internal server error for {UserName}");
//      }
//    }


    // PUT: api/City/5
    [HttpPut("UpdateCity/{CityId}")]
    public async Task<IActionResult> UpdateCity(int CityId,
                                                [FromBody] CityForUpdateDto CityForUpdateDto_Object,
                                                string UserName = "No Name",
                                                bool UseOwnMapster = false)
    {
      int NumberOfObjectsSaved = 0;
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

        var CityFromRepo = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

        if (null == CityFromRepo)
        {
          return NotFound();
        }

        if (!UseOwnMapster)
        {
          TypeAdapter.Adapt(CityForUpdateDto_Object, CityFromRepo);
        }
        else
        {
          CityFromRepo.MyMapsterCloneData<City>(CityForUpdateDto_Object);
        }

        //if (CityFromRepo.CloneData<City>(CityDto_Object))
        //{
          await _repositoryWrapper.CityRepositoryWrapper.Update(CityFromRepo);
#if Use_Hub_Logic_On_ServertSide
                await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif

        //}
        NumberOfObjectsSaved = await _repositoryWrapper.CityRepositoryWrapper.Save();

        if (1 == NumberOfObjectsSaved)
        {
          _logger.LogInfo($"City with Id : {CityFromRepo.CityId} has been updated by {UserName} !!!");
          return Ok($"City with Id : {CityFromRepo.CityId} has been updated by {UserName} !!!"); ;
        }
        else
        {
          _logger.LogError($"Error when updating City with Id : {CityFromRepo.CityId} by {UserName} !!!");
          return BadRequest($"Error when updating City with Id : {CityFromRepo.CityId} by {UserName} !!!");
        }
        //return NoContent();
      }
      catch (Exception Error)
      {
        _logger.LogError($"Something went wrong inside Update City action for {UserName}: {Error.Message}");
        return StatusCode(500, "Internal server error for {UserName}");
      }
    }

    //    [HttpPut("UpdateCityWithAllRelations/{CityId}")]
    //    public async Task<IActionResult> UpdateCityWithAllRelations(int CityId,
    //                                                                [FromBody] UpdateCityWithAllRelations UpdateCityWithAllRelations_Object,
    //                                                                bool DeleteOldElementsInListsNotSpecifiedInCurrentLists = true,
    //                                                                string UserName = "No Name")
    //    {
    //      try
    //      {
    //        List<int> AddedList = new List<int>();
    //        int ListCounter = 0;

    //        if (CityId != UpdateCityWithAllRelations_Object.CityDto_Object.CityId)
    //        {
    //          return BadRequest();
    //        }

    //        if (UpdateCityWithAllRelations_Object.CityDto_Object.CityDescription ==
    //            UpdateCityWithAllRelations_Object.CityDto_Object.CityName)
    //        {
    //          ModelState.AddModelError(
    //              "Description",
    //              "The provided description should be different from the name.");
    //        }

    //        if (!ModelState.IsValid)
    //        {
    //          _logger.LogError($"ModelState is Invalid for {UserName} in action UpdateCityWithAllRelations");
    //          return BadRequest(ModelState);
    //        }

    //        var CityFromRepo = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

    //        if (null == CityFromRepo)
    //        {
    //          return NotFound();
    //        }

    //        // Dur ikke med en Mapster Adapt i tilfældet med en update !!!
    //        // Derfor har jeg lavet min egen statiske metode CloneData til at kopiere 
    //        // data mellem 2 (generiske) objeter. Denne metode er lavet som en statisk metode i
    //        // en statisk klasse og kan derfor kaldes som en extension metode.
    //        // Metoden kan findes i filen Extensions/MyMapster.cs

    //        if (CityFromRepo.CloneData<City>(UpdateCityWithAllRelations_Object.CityDto_Object))
    //        {
    //          await _repositoryWrapper.CityRepositoryWrapper.Update(CityFromRepo);
    //#if Use_Hub_Logic_On_ServertSide
    //                await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
    //#endif

    //        }
    //        else
    //        {
    //          return BadRequest("Data Clone Fejl !!!");
    //        }

    //        if (null != UpdateCityWithAllRelations_Object.PointOfInterests)
    //        {
    //          for (int Counter = 0; Counter < UpdateCityWithAllRelations_Object.PointOfInterests.Count; Counter++)
    //          {
    //            if (0 == UpdateCityWithAllRelations_Object.PointOfInterests[Counter].PointOfInterestId)
    //            {
    //              PointOfInterestForSaveWithCityDto PointOfInterestForSaveWithCityDto_Object =
    //                  UpdateCityWithAllRelations_Object.PointOfInterests[Counter].Adapt<PointOfInterestForSaveWithCityDto>();

    //              var ActionResultAdd = await _pointOfInterestController.AddPointOfInterest(PointOfInterestForSaveWithCityDto_Object,
    //                                                                                        UserName);
    //              var OkResultActionResultAdd = ActionResultAdd as OkObjectResult;

    //              if (null != OkResultActionResultAdd)
    //              {
    //                int Test = (int)OkResultActionResultAdd.Value;
    //                AddedList.Add((int)OkResultActionResultAdd.Value);
    //              }
    //              else
    //              {
    //                string ErrorString = "Add Fejl i PointOfInterest objekt nummer " + Counter.ToString() + " !!!";
    //                return BadRequest(ErrorString);
    //              }
    //            }
    //            else
    //            {
    //              var ActionResultUpdate = await _pointOfInterestController.UpdatePointOfInterest(UpdateCityWithAllRelations_Object.PointOfInterests[Counter].PointOfInterestId,
    //                                                                                              UpdateCityWithAllRelations_Object.PointOfInterests[Counter],
    //                                                                                              UserName);
    //              var NoContentResultActionResultUpdate = ActionResultUpdate as NoContentResult;

    //              if (null == NoContentResultActionResultUpdate)
    //              {
    //                string ErrorString = "Update Fejl i PointOfInterest objekt nummer " + Counter.ToString() + " !!!";
    //                return BadRequest(ErrorString);
    //              }
    //            }
    //          }

    //          if (true == DeleteOldElementsInListsNotSpecifiedInCurrentLists)
    //          {
    //            var PointOfInterestList = await _repositoryWrapper.PointOfInterestRepositoryWrapper.GetAllPointOfInterestWithCityID(CityId, false);
    //            ListCounter = 1;

    //            foreach (PointOfInterest PointOfInterest_object in PointOfInterestList)
    //            {
    //              var Matches = UpdateCityWithAllRelations_Object.PointOfInterests.Where(p => p.PointOfInterestId == PointOfInterest_object.PointOfInterestId);
    //              if (0 == Matches.Count())
    //              {
    //                var Matches1 = AddedList.Any(p => p == PointOfInterest_object.PointOfInterestId);

    //                if (!Matches1)
    //                {
    //                  // Et af de nuværende PointOfinterests for det angivne CityId
    //                  // findes ikke i den nye liste over ønskede opdateringer og heller
    //                  // ikke i liste for nye PointOfInterests for det angivne CityId. 
    //                  // Og desuden er parameteren for at slette "gamle" elementer i
    //                  // PointOfInterest listen for det angivne CityId sat. Så slet 
    //                  // dette PointOfInterest fra databasen !!!
    //                  var ActionResultDelete = await _pointOfInterestController.DeletePointOfInterest(PointOfInterest_object.PointOfInterestId,
    //                                                                                                  UserName);
    //                  var NoContentActionResultDelete = ActionResultDelete as NoContentResult;

    //                  if (null == NoContentActionResultDelete)
    //                  {
    //                    string ErrorString = "Delete Fejl i PointOfInterest objekt nummer " + ListCounter.ToString() + " med PointOfInterestId " +
    //                                          PointOfInterest_object.PointOfInterestId.ToString() + " i Databasen !!!";
    //                    return BadRequest(ErrorString);
    //                  }
    //                }
    //              }
    //            }
    //          }
    //        }

    //        if (null != UpdateCityWithAllRelations_Object.CityLanguages)
    //        {
    //          var ActionResultUpdateCityLanguageList = await this._cityLanguageController.UpdateCityLanguagesList(UpdateCityWithAllRelations_Object.CityLanguages,
    //                                                                                                              DeleteOldElementsInListsNotSpecifiedInCurrentLists,
    //                                                                                                              UserName);
    //          var NoContentActionResultUpdateCityLanguageList = ActionResultUpdateCityLanguageList as NoContentResult;

    //          if (null == NoContentActionResultUpdateCityLanguageList)
    //          {
    //            var BadRequestActionResultUpdateCityLanguageList = ActionResultUpdateCityLanguageList as BadRequestObjectResult;

    //            string ErrorString = (string)(BadRequestActionResultUpdateCityLanguageList.Value);

    //            return BadRequest(ErrorString);
    //          }

    //        }

    //        return NoContent();
    //      }
    //      catch (Exception Error)
    //      {
    //        _logger.LogError($"Something went wrong inside action UpdateCityWithAllRelations for {UserName}: {Error.Message}");
    //        return StatusCode(500, "Internal server error for {UserName}");
    //      }
    //    }

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
          _logger.LogInfo(CommunicationResults_Object.ResultString);
        }
       
        return StatusCode(CommunicationResults_Object.HttpStatusCodeResult, CommunicationResults_Object.ResultString); 
      }
      catch (Exception Error)
      {
        _logger.LogError($"Something went wrong inside action UpdateCityWithAllRelations for {UserName}: {Error.Message}");
        return StatusCode(500, "Internal server error for {UserName}");
      }
    }

    // DELETE: api/5
    [HttpDelete("DeleteCity/{CityId}")]
    public async Task<IActionResult> DeleteCity(int CityId,
                                                string UserName = "No Name")
    {
      _repositoryWrapper.CityRepositoryWrapper.DisableLazyLoading();

      var CityFromRepo = await _repositoryWrapper.CityRepositoryWrapper.FindOne(CityId);

      if (null == CityFromRepo)
      {
        _logger.LogError($"City with Id {CityId} not found inside action DeleteCity for {UserName}");
        return NotFound();
      }

      await _repositoryWrapper.CityRepositoryWrapper.Delete(CityFromRepo);
#if Use_Hub_Logic_On_ServertSide
            await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
#endif
      _logger.LogInfo($"City with Id {CityId} has been deleted in action DeleteCity for {UserName}");
      return Ok($"City with Id {CityId} has been deleted in action DeleteCity for {UserName}");
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

            //CityDto CityDto_Object_Final = new CityDto();
            //CityDto_Object_Final.CityId = 0;
            //CityDto_Object_Final.CityName = "Egen Konvertering !!!";
            //CityDto_Object_Final.CityDescription = "Det sidste objekt her er lavet for at illustrere det arbejde, som AutoMapper gør for os !!!";

            //CityDtos.Add(CityDto_Object_Final);

            return (CityDtos);
        }
    }
}
