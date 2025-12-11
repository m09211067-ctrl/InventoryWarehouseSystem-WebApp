namespace InventoryWebApp.Patterns.Strategy
{
    public class QuantityDiscountStrategy : IPriceStrategy
    {
        public decimal CalculatePrice(decimal basePrice, int quantity)
        {
            if (quantity >= 10)
                return basePrice * 0.9m; // خصم 10%

            return basePrice;
        }
    }
}
