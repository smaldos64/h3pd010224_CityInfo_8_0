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
    public class PointOfInterestController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;

#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif
        public PointOfInterestController(ILoggerManager logger,
                                         IRepositoryWrapper repositoryWrapper)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
        }

        [HttpGet("GetPointOfInterests")]
        public async Task<IActionResult> GetPointOfInterests(string UserName = "No Name")
        {
            try
            {
                IEnumerable<PointOfInterest> PointOfInterestList = new List<PointOfInterest>();

                this._repositoryWrapper.PointOfInterestRepositoryWrapper.EnableLazyLoading();
                PointOfInterestList = await this._repositoryWrapper.PointOfInterestRepositoryWrapper.FindAll();

                List<PointOfInterestDto> PointOfInterestDtos;

                PointOfInterestDtos = PointOfInterestList.Adapt<PointOfInterestDto[]>().ToList();

                this._logger.LogInfo($"All PointOfInterests has been read from GetPointOfInterests action by {UserName}");
                return Ok(PointOfInterestDtos);
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside GetPointOfInterests action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        [HttpGet("GetPointOfInterest/{PointOfInterestsId}")]
        public async Task<IActionResult> GetPointOfInterest(int PointOfInterestsId,
                                                            string UserName = "No Name")
        {
            try
            {
                this._repositoryWrapper.PointOfInterestRepositoryWrapper.EnableLazyLoading();

                PointOfInterest PointOfInterest_Object = await this._repositoryWrapper.PointOfInterestRepositoryWrapper.FindOne(PointOfInterestsId);

                if (null == PointOfInterest_Object)
                {
                    return NotFound();
                }
                else
                {
                    PointOfInterestDto PointOfInterestDto_Object = PointOfInterest_Object.Adapt<PointOfInterestDto>();
                    return Ok(PointOfInterestDto_Object);
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetPointOfInterest action for {UserName} : {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // POST: api/PointOfInterest
        [HttpPost("CreatePointOfInterest")]
        public async Task<IActionResult> CreatePointOfInterest([FromBody] PointOfInterestForSaveDto PointOfInterestForSaveDto_Object,
                                                                string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsSaved = 0;
                
                PointOfInterest PointOfInterest_Object = PointOfInterestForSaveDto_Object.Adapt<PointOfInterest>();

                await this._repositoryWrapper.PointOfInterestRepositoryWrapper.Create(PointOfInterest_Object);
                NumberOfObjectsSaved = await this._repositoryWrapper.Save();

                if (1 == NumberOfObjectsSaved)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdatePointOfInterestDataMessage");
#endif
                    this._logger.LogInfo($"PointOfInterest with Id : {PointOfInterest_Object.PointOfInterestId} has been stored by {UserName} !!!");
                    return Ok(PointOfInterest_Object.PointOfInterestId);
                }
                else
                {
                    this._logger.LogError($"Error when saving PointOfInterest by {UserName} !!!");
                    return BadRequest($"Error when saving PointOfInterest_Object by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside CreatePointOfInterest action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // PUT: api/PointOfInterest/5
        [HttpPut("UpdatePointOfInterest/{PointOfInterestId}")]
        public async Task<IActionResult> UpdatePointOfInterest(int PointOfInterestId,
                                                    [FromBody] PointOfInterestForUpdateDto PointOfInterestForUpdateDto_Object,
                                                    string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsUpdated = 0;

                PointOfInterest PointOfInterest_Object = await this._repositoryWrapper.PointOfInterestRepositoryWrapper.FindOne(PointOfInterestId);

                if (null == PointOfInterest_Object)
                {
                    return NotFound();
                }

                TypeAdapter.Adapt(PointOfInterestForUpdateDto_Object, PointOfInterest_Object);

                await this._repositoryWrapper.PointOfInterestRepositoryWrapper.Update(PointOfInterest_Object);

                NumberOfObjectsUpdated = await this._repositoryWrapper.Save();

                if (1 == NumberOfObjectsUpdated)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdatePointOfInterestDataMessage");
#endif
                    this._logger.LogInfo($"PointOfInterest with Id : {PointOfInterest_Object.PointOfInterestId} has been updated by {UserName} !!!");
                    return Ok($"PointOfInterest with Id : {PointOfInterest_Object.PointOfInterestId} has been updated by {UserName} !!!"); ;
                }
                else
                {
                    this._logger.LogError($"Error when updating PointOfInterest with Id : {PointOfInterest_Object.PointOfInterestId} by {UserName} !!!");
                    return BadRequest($"Error when updating PointOfInterest with Id : {PointOfInterest_Object.PointOfInterestId} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside UpdatePointOfInterest action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }

        // DELETE: api/PointOfInterest/5
        [HttpDelete("DeletePointOfInterest/{PointOfInterestId}")]
        public async Task<IActionResult> DeletePointOfInterest(int PointOfInterestId,
                                                               string UserName = "No Name")
        {
            try
            {
                int NumberOfObjectsDeleted;

                PointOfInterest PointOfInterest_Object = await this._repositoryWrapper.PointOfInterestRepositoryWrapper.FindOne(PointOfInterestId);

                if (null == PointOfInterest_Object)
                {
                    this._logger.LogError($"PointOfInterest with Id {PointOfInterestId} not found inside action DeletePointOfInterest for {UserName}");
                    return NotFound();
                }

                await this._repositoryWrapper.PointOfInterestRepositoryWrapper.Delete(PointOfInterest_Object);

                NumberOfObjectsDeleted = await this._repositoryWrapper.Save();

                if (1 == NumberOfObjectsDeleted)
                {
#if Use_Hub_Logic_On_ServerSide
                    await this._broadcastHub.Clients.All.SendAsync("UpdatePointOfInterestDataMessage");
#endif
                    this._logger.LogInfo($"PointOfInterest with Id {PointOfInterestId} has been deleted in action DeletePointOfInterestId by {UserName}");
                    return Ok($"PointOfInterest with Id {PointOfInterestId} has been deleted in action DeletePointOfInterestId by {UserName}");
                }
                else
                {
                    this._logger.LogError($"Error when deleting PointOfInterest with Id : {PointOfInterestId} by {UserName} !!!");
                    return BadRequest($"Error when deleting PointOfInterest with Id : {PointOfInterestId} by {UserName} !!!");
                }
            }
            catch (Exception Error)
            {
                this._logger.LogError($"Something went wrong inside DeletePointOfInterest action for {UserName}: {Error.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            }
        }
    }
}
