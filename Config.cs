// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace ScarletAuth
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "roles",
                    UserClaims = { "role" }
                },
                new IdentityResources.Email
                {
                    Name = "email",
                    UserClaims = { "email", "email_verified" }
                },
                new IdentityResources.Address()
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource {
                    Name = "weatherapi",
                    Scopes = { "weatherapi.read", "weatherapi.write" },
                    ApiSecrets = { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = { "role" }
                },
                new ApiResource {
                    Name = "scarletauth.adminui_api",
                    Scopes = { "scarletauth.adminui_api" }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("weatherapi.read"),
                new ApiScope("weatherapi.write"),
                new ApiScope {
                    Name = "scarletauth.adminui_api",
                    DisplayName = "scarletauth.adminui_api",
                    Required = true,
                    UserClaims = {
                        "role",
                        "name"
                    }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "weatherapi.read" }
                },
                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:5021/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5021/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:5021/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "weatherapi.read" }
                },
                new Client
                {
                    ClientId = "scarletauth.adminui",
                    ClientName = "scarletauth.adminui",
                    ClientUri = "https://localhost:44303",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    ClientSecrets = { new Secret("A73560AE-8E8A-4508-80F6-B107EA362AB4".Sha256()) },
                    RedirectUris = { "https://localhost:44303/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44303/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44303/signout-callback-oidc" },
                    AllowedCorsOrigins = { "https://localhost:44303" },
                    AllowedScopes = { "openid", "email", "profile", "roles" }
                },
                new Client
                {
                    ClientId = "scarletauth.adminui_api_swaggerui",
                    ClientName = "scarletauth.adminui_api_swaggerui",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    RedirectUris = { "https://localhost:44302/swagger/oauth2-redirect.html" },
                    AllowedScopes = { "scarletauth.adminui_api" },
                    AllowedCorsOrigins = { "https://localhost:44302" }
                }
            };
    }
}