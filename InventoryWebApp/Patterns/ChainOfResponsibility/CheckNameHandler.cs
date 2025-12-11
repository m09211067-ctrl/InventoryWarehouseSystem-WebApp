using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.ChainOfResponsibility
{
    public class CheckNameHandler : BaseHandler
    {
        public override void Handle(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.ProductName))
                throw new Exception("Product name cannot be empty.");

            base.Handle(product);
        }
    }
}
