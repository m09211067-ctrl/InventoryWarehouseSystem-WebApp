using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.ChainOfResponsibility
{
    public class CheckPriceHandler : BaseHandler
    {
        public override void Handle(Product product)
        {
            if (product.Price <= 0)
                throw new Exception("❌ السعر يجب أن يكون أكبر من صفر");

            base.Handle(product);
        }
    }
}
