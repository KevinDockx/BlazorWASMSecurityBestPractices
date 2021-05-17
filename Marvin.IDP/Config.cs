// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Marvin.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()              
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope(
                    "myapi",
                    "My API Scope")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[] {
                new ApiResource(
                    "myapi", 
                    "My API")
                    {
                        Scopes = { "myapi" }
                    }
                };

        public static IEnumerable<Client> Clients => new Client[]
{
                new Client
                {
                    ClientId = "wasmclientimplicit",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    // for WASM/JavaScript, client secrets are useless 
                    // so don't require one
                    RequireClientSecret = false,
                    // RequirePkce = true,
                    // match the host of the WASM app
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:44322/authentication/login-callback"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44322/authentication/logout-callback"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new Client
                {
                    ClientId = "wasmclientcode",
                    AllowedGrantTypes = GrantTypes.Code,
                    // for WASM/JavaScript, client secrets are useless 
                    // so don't require one
                    RequireClientSecret = false,
                    RequirePkce = true,
                    // match the host of the WASM app
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:44322/authentication/login-callback"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44322/authentication/logout-callback"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "myapi"
                    }
                },
                new Client
                {
                    ClientId = "wasmbffcode",
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("secret".Sha256())
                    },
                    RequirePkce = true,
                    AllowOfflineAccess = true,
                    // match the BFF
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:44306/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44306/signout-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "myapi"
                    }
                }

};
    }
}