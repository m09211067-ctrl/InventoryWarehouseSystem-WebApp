using InventoryWebApp.Data;
using InventoryWebApp.Patterns;
using InventoryWebApp.Patterns.AbstractFactory;
using InventoryWebApp.Patterns.Adapter;


var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// ========== Database & Repositories ==========
builder.Services.AddTransient<DatabaseConnection>();

builder.Services.AddTransient<ProductRepository>();
builder.Services.AddTransient<WarehouseRepository>();
builder.Services.AddTransient<WarehouseStockRepository>();
builder.Services.AddTransient<MovementRepository>();

// ========== Abstract Factory ==========
builder.Services.AddTransient<IInventoryEntityFactory, DefaultInventoryFactory>();

// ========== Currency Adapter ==========
builder.Services.AddTransient<ExternalCurrencyAPI>();
builder.Services.AddTransient<ICurrencyService, CurrencyAdapter>();

// ========== Unit Of Work ==========
builder.Services.AddTransient<UnitOfWork>();

// ========== Facade ==========
builder.Services.AddTransient<InventoryFacade>();

builder.Services.AddTransient<WarehouseFacade>();


var app = builder.Build();

// ========== Pipeline ==========
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
