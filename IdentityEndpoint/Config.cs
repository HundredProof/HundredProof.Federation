// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityEndpoint
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };


        public static IEnumerable<Client> Clients =>
            new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "IDBROKER",
                    ClientSecrets = {new Secret("IDBROKER".Sha256())},
                    ClientName = "",
                    ClientUri = "https://localhost:5001",
                    ClientClaimsPrefix = "idbroker_",
                    Enabled = true,
                    RedirectUris = {"https://localhost:5001", "https://localhost:5001/signin-oidc"},
                    AllowedScopes = {"openid", "profile", "sub"},
                    AllowPlainTextPkce = false,
                    AlwaysSendClientClaims = true,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedGrantTypes = {
                        GrantType.AuthorizationCode
                    },
                    RequirePkce = true
                }
            };
    }
}