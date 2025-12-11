namespace InventoryWebApp.Patterns.Strategy
{
    public class IncreaseStockStrategy : IStockStrategy
    {
        public int Apply(int currentQuantity, int change)
        {
            return currentQuantity + change;
        }
    }
}
