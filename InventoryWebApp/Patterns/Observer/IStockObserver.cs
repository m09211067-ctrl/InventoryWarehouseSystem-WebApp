namespace InventoryWebApp.Patterns.Observer
{
    public interface IStockObserver
    {
        void Notify(string message);
    }
}
