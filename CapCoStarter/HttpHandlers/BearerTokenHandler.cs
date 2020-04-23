using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace CapCoStarter.HttpHandlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        readonly IHttpContextAccessor _httpContextAccessor;
        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //get the access token
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            //add token to request
            if (!string.IsNullOrWhiteSpace(accessToken))
                request.SetBearerToken(accessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
