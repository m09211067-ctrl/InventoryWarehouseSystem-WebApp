namespace InventoryWebApp.Patterns.Strategy
{
    public class BasicPriceStrategy : IPriceStrategy
    {
        public decimal CalculatePrice(decimal basePrice, int quantity)
        {
            return basePrice;
        }
    }
}
