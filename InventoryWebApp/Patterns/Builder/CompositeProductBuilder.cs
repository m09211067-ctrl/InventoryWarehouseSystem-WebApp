using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Builder
{
    public class CompositeProductBuilder : IProductBuilder
    {
        private Product _product;

        public CompositeProductBuilder()
        {
            _product = new Product();
            _product.Components = new List<string>();
        }

        public void SetName(string name)
        {
            _product.ProductName = name;
        }

        public void SetBarcode(string barcode)
        {
            _product.Barcode = barcode;
        }

        public void SetPrice(decimal price)
        {
            _product.Price = price;
        }

        public void SetQuantity(int qty)
        {
            _product.Quantity = qty;
        }

        public void AddComponent(string componentName)
        {
            _product.Components.Add(componentName);
        }

        public Product Build()
        {
            return _product;
        }
    }
}
