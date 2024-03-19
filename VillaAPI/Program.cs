using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using Serilog;
using VillaAPI.CustomLogging;

var builder = WebApplication.CreateBuilder(args);   // Here, the built-in Logger is used

// Add services to the container.

builder.Services.AddControllers(option =>
{
    option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Serilog
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File("log/villalogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Host.UseSerilog();    // Change the built-in logger

//builder.Services.AddScoped<ILogging, Logging>(); // This overrides the logger and uses the console only for itself 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
