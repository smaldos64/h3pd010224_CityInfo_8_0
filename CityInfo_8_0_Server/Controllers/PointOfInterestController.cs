using Mapster;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Contracts;
using ServicesContracts;
using CityInfo_8_0_Server.Extensions;
using Entities.DataTransferObjects;
using Entities.Models;
using Services;

#if Use_Hub_Logic_On_ServertSide
using CityInf0_8_0_Server.Hubs;
#endif

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInf0_8_0_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointOfInterestController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;
        private IPointOfInterestService _pointOfInterestService;

#if Use_Hub_Logic_On_ServertSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif

        public PointOfInterestController(ILoggerManager logger,
                                         IRepositoryWrapper repositoryWrapper,
                                         IPointOfInterestService pointOfInterestService)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
            this._pointOfInterestService = pointOfInterestService;
        }

        // PUT api/<PointOfInteresController>/5
        [HttpPut("{CityId}")]
        public async Task<IActionResult> UpdatePointOfInterestListForCity(int CityId,
                                                                        [FromBody] List<PointOfInterestForUpdateDto> PointOfInterestForUpdateDto_List,
                                                                        bool DeleteOldElementsInListNotSpecifiedInCurrentList = true,
                                                                        string UserName = "No Name",
                                                                        bool UseExtendedDatabaseDebugging = false)
        {
            try
            {
                ICommunicationResults CommunicationResults_Object;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                CommunicationResults_Object = await _pointOfInterestService.UpdatePointOfInterestListForCity(CityId,
                                                                                                PointOfInterestForUpdateDto_List,
                                                                                                DeleteOldElementsInListNotSpecifiedInCurrentList,
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
                _logger.LogError($"Something went wrong inside action UpdatePointOfInterestListForCity for {UserName}: {Error.Message}");
                return StatusCode(500, "Internal server error for {UserName}");
            }
        }

        //        // GET: api/<PointOfInterestController>
        //        [HttpGet]
        //        //public async Task<IActionResult> PointOfInterestDto(bool includeRelations = false)
        //        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterests(bool includeRelations = true,
        //                                                                                             string UserName = "No Name")
        //        {
        //            if (false == includeRelations)
        //            {
        //                _repositoryWrapper.LanguageRepositoryWrapper.DisableLazyLoading();
        //            }
        //            else  // true == includeRelations 
        //            {
        //                _repositoryWrapper.LanguageRepositoryWrapper.EnableLazyLoading();
        //            }

        //            var PointOfInterestList = await _repositoryWrapper.PointOfInterestRepositoryWrapper.FindAll();

        //            List<PointOfInterestDto> PointOfInterestDtos = PointOfInterestList.Adapt<PointOfInterestDto[]>().ToList();

        //#if Test_Logging
        //            this._logger.LogWarning(PointOfInterestControllerLoggingEventID, "Alle PointOfInterests læst af : " + UserName);
        //#endif

        //            return Ok(PointOfInterestDtos);
        //        }

        //        // GET api/<PointOfInterestController>/5
        //        [HttpGet("{id}", Name = "GetPointOfInterest")]
        //        //public async Task<IActionResult> GetPointOfInterest(int id, bool includeRelations = false)
        //        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int Id, 
        //                                                                               bool includeRelations = true,
        //                                                                               string UserName = "No Name")
        //        {
        //            if (false == includeRelations)
        //            {
        //                _repositoryWrapper.LanguageRepositoryWrapper.DisableLazyLoading();
        //            }
        //            else  // true == includeRelations 
        //            {
        //                _repositoryWrapper.LanguageRepositoryWrapper.EnableLazyLoading();
        //            }

        //            var PointOfInterest_Object = await _repositoryWrapper.PointOfInterestRepositoryWrapper.FindOne(Id);

        //            PointOfInterestDto PointOfInterestDto_Object = new PointOfInterestDto();

        //#if Test_Logging
        //            this._logger.LogWarning(PointOfInterestControllerLoggingEventID, "PointOfInterest med PointOfInterestId : " + Id.ToString() + " læst af : " + UserName);
        //#endif

        //            return Ok(PointOfInterestDto_Object = PointOfInterest_Object.Adapt<PointOfInterestDto>());
        //        }

        //        // POST api/<PointOfInteresController>
        //        [HttpPost]
        //        public async Task<IActionResult> AddPointOfInterest([FromBody] PointOfInterestForSaveWithCityDto PointOfInterestDto_Object,
        //                                                            string UserName = "No Name")
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return BadRequest(ModelState);
        //            }

        //            PointOfInterest PointOfInterest_Object = PointOfInterestDto_Object.Adapt<PointOfInterest>();
        //            await _repositoryWrapper.PointOfInterestRepositoryWrapper.Create(PointOfInterest_Object);
        //#if Use_Hub_Logic_On_ServertSide
        //            await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
        //#endif

        //#if Test_Logging
        //            this._logger.LogWarning(PointOfInterestControllerLoggingEventID, "PointOfInterest med PointOfInterestId : " + PointOfInterest_Object.PointOfInterestId.ToString() +
        //                                    " og navn : " + PointOfInterest_Object.Name + " oprettet af : " + UserName);
        //#endif

        //            return Ok(PointOfInterest_Object.PointOfInterestId);
        //        }

        //        // PUT api/<PointOfInteresController>/5
        //        [HttpPut("{id}")]
        //        public async Task<IActionResult> UpdatePointOfInterest(int Id, 
        //                                                               [FromBody] PointOfInterestForUpdateDto PointOfInterestDto_Object,
        //                                                               string UserName = "No Name")
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return BadRequest(ModelState);
        //            }

        //            var PointOfInterestFromRepo = await _repositoryWrapper.PointOfInterestRepositoryWrapper.FindOne(Id);

        //            if (null == PointOfInterestFromRepo)
        //            {
        //                return NotFound();
        //            }

        //            if (PointOfInterestFromRepo.CloneData<PointOfInterest>(PointOfInterestDto_Object))
        //            {
        //                await _repositoryWrapper.PointOfInterestRepositoryWrapper.Update(PointOfInterestFromRepo);
        //#if Use_Hub_Logic_On_ServertSide
        //                await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
        //#endif

        //#if Test_Logging
        //                this._logger.LogWarning(PointOfInterestControllerLoggingEventID, "PointOfInterest med PointOfInterestId : " + PointOfInterestDto_Object.PointOfInterestId.ToString() + " ændret til (Name) : " +
        //                                        PointOfInterestDto_Object.Name + " (Description) : " +
        //                                        PointOfInterestDto_Object.Description + " (CityId) : " +
        //                                        PointOfInterestDto_Object.CityId + " ændret af : " + UserName);
        //#endif
        //            }

        //            return NoContent();
        //        }

        //        // DELETE api/<PointOfInterestController>/5
        //        [HttpDelete("{id}")]
        //        public async Task<IActionResult> DeletePointOfInterest(int Id,
        //                                                               string UserName = "No Name")
        //        {
        //            _repositoryWrapper.CityInfoRepositoryWrapper.DisableLazyLoading();

        //            var PointOfInterestFromRepo = await _repositoryWrapper.PointOfInterestRepositoryWrapper.FindOne(Id);

        //            if (null == PointOfInterestFromRepo)
        //            {
        //                return NotFound();
        //            }

        //            await _repositoryWrapper.PointOfInterestRepositoryWrapper.Delete(PointOfInterestFromRepo);
        //#if Use_Hub_Logic_On_ServertSide
        //            await this._broadcastHub.Clients.All.SendAsync("UpdateCityDataMessage");
        //#endif

        //#if Test_Logging
        //            this._logger.LogWarning(PointOfInterestControllerLoggingEventID, "PointOfInterest med PointOfInterestId : " + Id.ToString() + " slettet af : " + UserName);
        //#endif

        //            return NoContent();
        //        }
    }
}
