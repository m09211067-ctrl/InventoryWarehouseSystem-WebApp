using InventoryWebApp.Patterns.Singleton;

namespace InventoryWebApp.Patterns.Observer
{
    public class LowStockObserver : IStockObserver
    {
        public void Notify(string message)
        {
            LogManager.Instance.Write("[Low Stock] " + message);
        }
    }
}
