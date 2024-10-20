using WareHouse.Services.OrderService;
using WarehouseManagementAPI;

var builder = WebApplication.CreateBuilder(args);

string filePath = "C:\\Users\\Ibrohim\\Desktop\\WarehouseManagementAPI\\WarehouseManagementAPI\\appsettings.json";

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(filePath, optional: false, reloadOnChange: true)
    .Build();


builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddApplicationServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.Run();

