
using Microsoft.AspNetCore.Mvc;
using ABCREPORTSYSTEM.Sucursal.Models;
using ABCREPORTSYSTEM.Sucursal.Services;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

// Agregar la conexi�n a la base de datos a la colecci�n de servicios
builder.Services.AddSqlServer<AbcdataBaseContext>(builder.Configuration.GetConnectionString("ConnectionABC"));

// Agregar el controlador de MVC a la colecci�n de servicios
builder.Services.AddScoped<IAutomobiles, Automobiles>();
builder.Services.AddScoped<ISucursal,Sucursal>();
builder.Services.AddScoped<IEmpleado, Empleado>();
builder.Services.AddControllers();

// Agregar las p�ginas Razor a la colecci�n de servicios
builder.Services.AddRazorPages();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapControllers();
Console.WriteLine("HOLA MUNDO");
app.Run();
