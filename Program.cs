using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/getall", (IConfiguration _configuration) =>
{
    var configRoot = _configuration as IConfigurationRoot;

    if (configRoot == null)
        return Results.Json("ConfigurationRoot is not available");

    var provider = configRoot.Providers.FirstOrDefault(p => p.ToString().Contains("JsonConfigurationProvider"));

    var settings = GetSettingsFromProvider(provider, _configuration);

    var source = new
    {
        Provider = provider.ToString(),
        Settings = settings
    };

    return Results.Json(source);
})
.WithName("GetAllAppsettings")
.WithOpenApi();

Dictionary<string,string> GetSettingsFromProvider(IConfigurationProvider provider, IConfiguration _configuration)
{
    var settings = new Dictionary<string, string>();

    foreach (var kvp in _configuration.AsEnumerable().OrderBy(kvp => kvp.Key))
    {
        if (provider.TryGet(kvp.Key, out var value))
        {
            settings[kvp.Key] = value;
        }
    }

    return settings;
}

app.Run();

