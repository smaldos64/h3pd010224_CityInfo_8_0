using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo_8_0_Server.Controllers
{
    // Koden her skal bruges til at lave en fuld generisk Controller. Jeg er lige startet
    // på at lave koden som i lige startet. Planen er, at koden i controlleren her er 
    // færdig og fuldt funktinsdygtig i løbet af et par SW releases.

    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : ControllerBase where T : class, new()
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;

#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif
        public GenericController(ILoggerManager logger,
                                 IRepositoryWrapper repositoryWrapper)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
        }

        // GET: api/<LanguageController>
        [HttpGet("GetData")]
        public async Task<IActionResult> GetDataById(int Id,
                                                     string UserName = "No Name")
        {
            //try
            //{
            //    dynamic DataList;

            //    _repositoryWrapper.LanguageRepositoryWrapper.EnableLazyLoading();

            //    var entity = 
            //    if (0 == DataType) 
            //    {
            //        DataList = await _repositoryWrapper.LanguageRepositoryWrapper.FindAll();
            //    }
            //    else
            //    {
            //        DataList = await _repositoryWrapper.CityRepositoryWrapper.FindAll();
            //    }
            //    //IEnumerable<Language> LanguageList = new List<Language>();

            //    //_repositoryWrapper.LanguageRepositoryWrapper.EnableLazyLoading();
            //    //LanguageList = await _repositoryWrapper.LanguageRepositoryWrapper.FindAll();

            //    List<LanguageDto> LanguageDtos;

            //    LanguageDtos = LanguageList.Adapt<LanguageDto[]>().ToList();

            //    _logger.LogInfo($"All Languages has been read from GetLanguages action by {UserName}");
            //    return Ok(LanguageDtos);
            //}
            //catch (Exception Error)
            //{
            //    _logger.LogError($"Something went wrong inside GetLanguages action for {UserName} : {Error.Message}");
            //    return StatusCode((int)HttpStatusCode.InternalServerError, $"Internal server error : {Error.ToString()}");
            //}

            return Ok();
        }
    }
}
