using IdentityServer4;
using IdentityServer4.Models;

namespace Identity.API.Configuration;

public static class Config
{
    // Identity resources are data like user ID, name, or email address of a user
    public static IEnumerable<IdentityResource> GetResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
    }

    // ApiResources define the apis in your system
    public static IEnumerable<ApiResource> GetApis()
    {
        return new List<ApiResource>
        {
            new ApiResource("usermanagment", "User Managment Service"),
            new ApiResource("dictionary", "Dictionary Service"),
            new ApiResource("testing", "Testing Service"),
            new ApiResource("testingagg", "Testing Aggregator")
        };
    }

    public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "js",
                ClientName = "Angular Client",
                AllowedGrantTypes = new List<string> { GrantType.ResourceOwnerPassword },
                RequireClientSecret = false,
                RequireConsent = false,
                RedirectUris = new List<string> { $"{clientsUrl["Spa"]}/signin-callback.html" },
                PostLogoutRedirectUris = new List<string> { clientsUrl["Spa"] },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "usermanagment",
                    "dictionary",
                    "testing",
                    "testingagg"
                },
                AllowedCorsOrigins = new List<string>
                {
                    clientsUrl["Spa"],
                }
            }
        };
    }
}
