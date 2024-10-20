using WareHouse.Services.OrderService;

namespace WarehouseManagementAPI;



public static class ServiceExtensions
{
    
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IOrderService, OrderService>();
    }
}