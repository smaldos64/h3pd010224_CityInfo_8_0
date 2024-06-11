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

#if Use_Hub_Logic_On_ServerSide
using CityInf0_8_0_Server.Hubs;
#endif

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInf0_8_0_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointOfInterestEducationController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;
        private IPointOfInterestService _pointOfInterestService;

#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif

        public PointOfInterestEducationController(ILoggerManager logger,
                                         IRepositoryWrapper repositoryWrapper,
                                         IPointOfInterestService pointOfInterestService)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
            this._pointOfInterestService = pointOfInterestService;
        }

        // PUT api/<PointOfInteresController>/5
        [HttpPut("UpdatePointOfInterestListForCity/{CityId}")]
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
    }
}
