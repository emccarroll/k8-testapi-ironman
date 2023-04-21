using System.Diagnostics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
        tracerProviderBuilder
            .ConfigureResource(resource => resource
                .AddService(serviceName: "IronManService"))
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var httpClient = new HttpClient();

app.MapGet("/api/ironman/slogan", () => "I am Iron Man")
.WithName("GetIronmanSlogan");

app.MapGet("/api/ironman/web", async () => {
    var html = await httpClient.GetStringAsync("http://localhost:5000/api/ironman/slogan");
    if(string.IsNullOrWhiteSpace(html))
        return "empty response";
    return html;
})
.WithName("GetIronmanWeb");

app.Run();