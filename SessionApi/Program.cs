using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SessionApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

builder.Services.RegisterApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Services.InitializeDbContext();
app.MapHealthChecks("/health", new()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponseNoExceptionDetails
});
app.UseAuthorization();
app.MapControllers();

app.UseHttpLogging();

app.Run();