using System.Collections.Generic;

namespace InventoryWebApp.Patterns.Observer
{
    public class StockSubject
    {
        private static readonly StockSubject _instance = new();
        public static StockSubject Instance => _instance;

        private readonly List<IStockObserver> _observers = new();

        private StockSubject() { }

        public void Attach(IStockObserver observer) => _observers.Add(observer);
        public void Detach(IStockObserver observer) => _observers.Remove(observer);

        public void Notify(string message)
        {
            foreach (var obs in _observers)
                obs.Notify(message);
        }
    }
}
