using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Builder
{
    public class ProductDirector
    {
        public Product BuildComputer(IProductBuilder builder)
        {
            builder.SetName("Computer Set");
            builder.SetBarcode("CMP-001");
            builder.SetPrice(3500);
            builder.SetQuantity(1);

            builder.AddComponent("CPU");
            builder.AddComponent("RAM");
            builder.AddComponent("SSD");

            return builder.Build();
        }
    }
}
