using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Identity.API.Configuration;

namespace Identity.API.Data;

public class ConfigurationDbContextSeed
{
    public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
    {

        //callbacks urls from config:
        var clientUrls = new Dictionary<string, string>();

        clientUrls.Add("DictionaryApi", configuration["DictionaryApiClient"]!);

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.GetResources())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.ApiScopes.Any())
        {
            foreach (var api in Config.GetApiScopes())
            {
                context.ApiScopes.Add(api.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var api in Config.GetApis())
            {
                context.ApiResources.Add(api.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.Clients.Any())
        {
            foreach (var client in Config.GetClients(clientUrls))
            {
                context.Clients.Add(client.ToEntity());
            }
            await context.SaveChangesAsync();
        }
    }
}
