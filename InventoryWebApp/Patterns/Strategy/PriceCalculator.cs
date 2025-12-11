namespace InventoryWebApp.Patterns.Strategy
{
    public class PriceCalculator
    {
        private IPriceStrategy _strategy;

        public PriceCalculator(IPriceStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IPriceStrategy strategy)
        {
            _strategy = strategy;
        }

        public decimal Calculate(decimal basePrice, int quantity)
        {
            return _strategy.CalculatePrice(basePrice, quantity);
        }
    }
}
