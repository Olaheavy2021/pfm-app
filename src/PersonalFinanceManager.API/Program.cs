var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

if (builder.Environment.IsDevelopment())
{
    config.AddUserSecrets<IApiMarker>();
}

Log.Logger.ConfigureSerilogBootstrapLogger();

builder.AddServiceDefaults();

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddServiceCollections(config);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(
    "v1",
    options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>()
);

var app = builder.Build();

await app.UseMyAppDefaultsAsync();

app.Run();
