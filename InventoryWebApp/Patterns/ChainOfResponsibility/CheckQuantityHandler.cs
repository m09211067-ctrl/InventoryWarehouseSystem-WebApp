using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.ChainOfResponsibility
{
    public class CheckQuantityHandler : BaseHandler
    {
        public override void Handle(Product product)
        {
            if (product.Quantity < 0)
                throw new Exception("Quantity cannot be negative.");

            base.Handle(product);
        }
    }
}
