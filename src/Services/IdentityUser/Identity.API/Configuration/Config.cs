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
    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("usermanagment", "User Managment Service"),
            new ApiScope("dictionary", "Dictionary Service"),
            new ApiScope("testing", "Testing Service"),
            new ApiScope("testingagg", "Testing Aggregator")
        };
    }

    public static IEnumerable<ApiResource> GetApis()
    {
        return new List<ApiResource>
        {
            new ApiResource("ApiName")
            {
                ApiSecrets = {
                    new Secret("secret_for_the_api".Sha256())
                }
            }
        };
    }

    public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "spa.client",
                ClientName = "Client Credentials Client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    //"usermanagment",
                    //"dictionary",
                    //"testing",
                    //"testingagg"
                    "ApiName"
                }
            }
        };
    }
}
