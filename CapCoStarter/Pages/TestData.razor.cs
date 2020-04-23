using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using CapCoStarter.Data;
using CapCoStarter.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapCoStarter.Pages
{
    public class TestDataBase : ComponentBase
    {
        [CascadingParameter]
        Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public ITestDataService TestDataService { get; set; }


        public TestDataValue TestDataValue { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await authenticationStateTask;            
            if (authenticationState.User.Identity.IsAuthenticated && !string.IsNullOrWhiteSpace(authenticationState.User.Claims.FirstOrDefault(c => c.Type == "name").Value))
            {
                var tdv = await TestDataService.GetSecureDataAsync();
                tdv.TestValue += authenticationState.User.Claims.FirstOrDefault(c => c.Type == "name").Value;
                TestDataValue = tdv;
            }
            else
            {
                var tdv = await TestDataService.GetUnsecuredDataAsync();
                tdv.TestValue += " Not signed in";
                TestDataValue = tdv;
            }
            //await base.OnInitializedAsync();
            await InvokeAsync(StateHasChanged);
        }
    }
}
