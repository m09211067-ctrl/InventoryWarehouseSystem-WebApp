using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Prototype
{
    public interface ICloneableProduct
    {
        Product Clone();
    }
}
