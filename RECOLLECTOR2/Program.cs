using RECOLLECTOR2;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<RECOLECTOR>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();