namespace InventoryWebApp.Patterns.Strategy
{
    public class StockCalculator
    {
        private IStockStrategy _strategy;

        public StockCalculator(IStockStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IStockStrategy strategy)
        {
            _strategy = strategy;
        }

        public int Execute(int currentQty, int change)
        {
            return _strategy.Apply(currentQty, change);
        }
    }
}
