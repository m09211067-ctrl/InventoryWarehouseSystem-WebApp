namespace InventoryWebApp.Patterns.Decorator
{
    public class BasePrice : IPriceComponent
    {
        private readonly decimal _price;

        public BasePrice(decimal price)
        {
            _price = price;
        }

        public decimal GetPrice() => _price;
    }
}
