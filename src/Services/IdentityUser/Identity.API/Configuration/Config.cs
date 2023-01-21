using Duende.IdentityServer.Models;

namespace Identity.API.Configuration;

public static class Config
{
    public static IEnumerable<IdentityResource> GetResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResource("roles", new[] { "role" })
        };
    }

    public static IEnumerable<ApiResource> GetApis()
    {
        return new List<ApiResource>
        {
            new ApiResource {
                Name = "usermanagement",
                DisplayName = "User Management Service",
                Scopes = new List<string> {
                    "usermanagement"
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
            new ApiScope("usermanagement") { UserClaims = new[] { "role" } },
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
                ClientId = "usermanagement-swagger",
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { $"{clientsUrl["UserManagementApi"]}/swagger/oauth2-redirect.html" },
                AllowedCorsOrigins = { clientsUrl["UserManagementApi"] },
                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "roles", "usermanagement" },
            },
            new Client
            {
                ClientId = "dictionary-swagger",
                RequireClientSecret = false,
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { $"{clientsUrl["DictionaryApi"]}/swagger/oauth2-redirect.html" },
                AllowedCorsOrigins = { clientsUrl["DictionaryApi"] },
                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "roles", "dictionary" },
            }
        };
    }
}
