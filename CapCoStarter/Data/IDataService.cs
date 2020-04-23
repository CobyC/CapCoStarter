using CapCoStarter.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CapCoStarter.Data
{
    public interface ITestDataService
    {
        Task<TestDataValue> GetSecureDataAsync();
        Task<TestDataValue> GetUnsecuredDataAsync();
    }
}
