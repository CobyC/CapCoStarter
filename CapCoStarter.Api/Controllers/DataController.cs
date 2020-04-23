using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using CapCoStarter.Shared;

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