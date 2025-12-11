namespace InventoryWebApp.Patterns.Strategy
{
    public interface IPriceStrategy
    {
        decimal CalculatePrice(decimal basePrice, int quantity);
    }
}
