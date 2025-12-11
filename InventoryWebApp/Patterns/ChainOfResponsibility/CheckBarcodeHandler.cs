using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.ChainOfResponsibility
{
    public class CheckBarcodeHandler : BaseHandler
    {
        public override void Handle(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Barcode))
                throw new Exception("Barcode cannot be empty.");

            base.Handle(product);
        }
    }
}
