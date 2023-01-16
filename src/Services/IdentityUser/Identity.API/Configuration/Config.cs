﻿using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity.API.Configuration;

public static class Config
{
    public static IEnumerable<IdentityResource> GetResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("roles", new[] { "role" })
        };
    }

    public static IEnumerable<ApiResource> GetApis()
    {
        return new List<ApiResource>
        {
            new ApiResource {
                Name = "usermanagment",
                DisplayName = "User Managment Service",
                Scopes = new List<string> {
                    "usermanagment"
                }
            },
            new ApiResource {
                Name = "dictionary",
                DisplayName = "Dictionary Service",
                Scopes = new List<string> {
                    "dictionary"
                }
            },
            new ApiResource {
                Name = "testing",
                DisplayName = "Testing Service",
                Scopes = new List<string> {
                    "testing"
                }
            },
            new ApiResource {
                Name = "testingagg",
                DisplayName = "Testing Aggregator",
                Scopes = new List<string> {
                    "testingagg"
                }
            },
        };
    }

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("usermanagment") { UserClaims = new[] { "role" } },
            new ApiScope("dictionary") { UserClaims = new[] { "role" } },
            new ApiScope("testing") { UserClaims = new[] { "role" } },
            new ApiScope("testingagg") { UserClaims = new[] { "role" } },
        };
    }

    public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "spa.client",
                ClientName = "Spa Client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                AllowedCorsOrigins = { clientsUrl["DictionaryApi"] },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "usermanagment",
                    "dictionary",
                    "testing",
                    "testingagg",
                    "roles"
                },
            }
        };
    }
}
