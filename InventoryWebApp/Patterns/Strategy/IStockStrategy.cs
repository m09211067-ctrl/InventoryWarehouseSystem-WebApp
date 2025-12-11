namespace InventoryWebApp.Patterns.Strategy
{
    public interface IStockStrategy
    {
        int Apply(int currentQuantity, int change);
    }
}
