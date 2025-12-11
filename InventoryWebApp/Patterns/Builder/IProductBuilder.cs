using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Builder
{
    public interface IProductBuilder
    {
        void SetName(string name);
        void SetBarcode(string barcode);
        void SetPrice(decimal price);
        void SetQuantity(int qty);
        void AddComponent(string componentName);
        Product Build();
    }
}
