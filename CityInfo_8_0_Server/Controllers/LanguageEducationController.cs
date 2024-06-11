using Contracts;
using Entities.Models;
using Entities.DataTransferObjects;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ServicesContracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo_8_0_Server.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LanguageEducationController : ControllerBase
  {
    private IRepositoryWrapper _repositoryWrapper;
    private ILoggerManager _logger;

    public LanguageEducationController(ILoggerManager logger,
                              IRepositoryWrapper repository)
    {
      this._logger = logger;
      this._repositoryWrapper = repository;
    }

    // GET: api/<LanguageController>
    [HttpGet("GetLanguages")]
    public async Task<ActionResult<IEnumerable<LanguageDto>>> GetLanguages(bool includeRelations = true,
                                                                           string UserName = "No Name")
    {
      try
      {
        if (false == includeRelations)
        {
          _repositoryWrapper.LanguageRepositoryWrapper.DisableLazyLoading();
        }
        else  // true == includeRelations 
        {
          _repositoryWrapper.LanguageRepositoryWrapper.EnableLazyLoading();
        }

        var LanguageList = await _repositoryWrapper.LanguageRepositoryWrapper.FindAll();

        List<LanguageDto> LanguageDtos = LanguageList.Adapt<LanguageDto[]>().ToList();

        _logger.LogInfo("All Languages has been read from GetLanguages action");

        return Ok(LanguageDtos);
      }
      catch (Exception Error)
      {
        _logger.LogError($"Something went wrong inside GetLanguages action: {Error.Message}");
        return StatusCode(500, "Internal server error");
      }
    }
  }
}
