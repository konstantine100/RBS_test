using RBS.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure pipeline
app.ConfigureRequestPipeline();

app.Run();