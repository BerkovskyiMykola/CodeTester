﻿using Identity.API.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace Identity.API.Data;

public class ConfigurationDbContextSeed
{
    public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
    {

        //callbacks urls from config:
        var clientUrls = new Dictionary<string, string>();

        clientUrls.Add("Spa", configuration.GetValue<string>("SpaClient") ?? "");

        if (!context.Clients.Any())
        {
            foreach (var client in Config.GetClients(clientUrls))
            {
                context.Clients.Add(client.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.GetResources())
            {
                context.IdentityResources.Add(resource.ToEntity());
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
    }
}
