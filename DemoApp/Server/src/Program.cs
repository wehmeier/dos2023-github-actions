using FP.GitHubActions.DemoApp.Server.Business;
using FP.GitHubActions.DemoApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<AccountRepository>();
builder.Services.AddConnections();

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<DemoAppServices>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}");

app.Run();