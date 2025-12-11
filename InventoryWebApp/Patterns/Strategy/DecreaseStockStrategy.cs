namespace InventoryWebApp.Patterns.Strategy
{
    public class DecreaseStockStrategy : IStockStrategy
    {
        public int Apply(int currentQuantity, int change)
        {
            return currentQuantity - change;
        }
    }
}
