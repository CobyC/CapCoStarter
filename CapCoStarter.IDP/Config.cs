// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Collections.Generic;

namespace CapCoStarter.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Email(),
                new IdentityResource("roles","User role(s)",
                    new List<string>{ "role" })
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("CapCoStarterapi",
                    "PSL Monitor API",
                    new[]{ "roles" }),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client{
                    ClientName = "PSL Monitor Website",
                    ClientId = "CapCoStarterclient",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    //RequirePkce = true,
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:44317/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44317/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "CapCoStarterapi"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowOfflineAccess = true
            }};

    }
}