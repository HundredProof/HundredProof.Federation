﻿using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityEndpoint {
    public static class Config {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[] {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };


        public static IEnumerable<ApiResource> Apis =>
            new[] {
                new ApiResource("api1", "My API #1")
            };


        public static IEnumerable<Client> Clients =>
            new[] {
                // client credentials flow client
                new Client {
                    ClientId = "IDBROKER",
                    ClientSecrets = {new Secret("IDBROKER".Sha256())},
                    ClientName = "",
                    ClientUri = "https://broker.example-1.getthinktank.com",
                    ClientClaimsPrefix = "idbroker_",
                    Enabled = true,
                    RedirectUris = {
                        "https://broker.example-1.getthinktank.com",
                        "https://broker.example-1.getthinktank.com/signin-oidc"
                    },
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