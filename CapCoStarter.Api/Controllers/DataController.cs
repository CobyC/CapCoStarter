using CapCoStarter.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CapCoStarter.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly ILogger<TestDataValue> _logger;
        public DataController(ILogger<TestDataValue> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("getsecureddata")]
        [Authorize]
        public async Task<TestDataValue> GetSecuredData()
        {
            return await Task.FromResult(new TestDataValue { TestValue = "Api secured available." });
        }

        [HttpGet]
        [Route("getunsecureddata")]
        public Task<TestDataValue> GetUnsecuredData()
        {
            return Task.FromResult(new TestDataValue() { TestValue = "Api aunsecured available." });
        }
    }
}