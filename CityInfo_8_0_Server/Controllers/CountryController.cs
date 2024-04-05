using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using ServicesContracts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo_8_0_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private IRepositoryWrapper _repositoryWrapper;
        private ILoggerManager _logger;
        
#if Use_Hub_Logic_On_ServerSide
        private readonly IHubContext<BroadcastHub> _broadcastHub;
#endif
        public CountryController(ILoggerManager logger,
                              IRepositoryWrapper repositoryWrapper)
        {
            this._logger = logger;
            this._repositoryWrapper = repositoryWrapper;
        }

        // GET: api/<CountryController>
        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries(string UserName = "No Name")
        {
            try
            {
                IEnumerable<Country> CountryList = new List<Country>();

                _repositoryWrapper.CityRepositoryWrapper.EnableLazyLoading();
                CountryList = await _repositoryWrapper.CountryRepositoryWrapper.FindAll();

                List<CountryDto> CountryDtos;

                CountryDtos = CountryList.Adapt<CountryDto[]>().ToList();

                _logger.LogInfo($"All Countries has been read from GetCountries action by {UserName}");
                return Ok(CountryDtos);
            }
            catch (Exception Error)
            {
                _logger.LogError($"Something went wrong inside GetCities action for {UserName} : {Error.Message}");
                return StatusCode(500, $"Internal server error : {Error.ToString()}");
            }
        }

        // GET api/<CountryController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CountryController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CountryController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CountryController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
