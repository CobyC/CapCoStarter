using CapCoStarter.Shared;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CapCoStarter.Data
{
    public class TestDataService : ITestDataService
    {

        private readonly HttpClient _HttpClient;
        public TestDataService(HttpClient httpClient)
        {
            _HttpClient = httpClient;
        }
        public async Task<TestDataValue> GetSecureDataAsync()
        {
            return await JsonSerializer.DeserializeAsync<TestDataValue>(
                await _HttpClient.GetStreamAsync($"api/data/getsecureddata"),
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                //new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });//this required the property names to match exactly
        }

        public async Task<TestDataValue> GetUnsecuredDataAsync()
        {

            TestDataValue rVal = await JsonSerializer.DeserializeAsync<TestDataValue>(
                await _HttpClient.GetStreamAsync($"api/data/getunsecureddata"),
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return rVal;
        }
    }
}
