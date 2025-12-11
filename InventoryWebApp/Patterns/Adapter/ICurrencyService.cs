namespace InventoryWebApp.Patterns.Adapter
{
    public interface ICurrencyService
    {
        decimal GetExchangeRate(); // USD → LYD
    }
}
