using Dictionary.API;
using Dictionary.API.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DictionaryDBContext>(options =>
{
    options.UseNpgsql(builder.Configuration["ConnectionString"], sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
    });
    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dictionary.API");
    });
}

app.UseAuthorization();

app.MapControllers();

app.MigrateDbContext<DictionaryDBContext>((context, services) =>
{
    var logger = services.GetRequiredService<ILogger<DictionaryDBContext>>();

    new DictionaryDBContextSeed()
        .SeedAsync(context, logger)
        .Wait();
});

app.Run();
