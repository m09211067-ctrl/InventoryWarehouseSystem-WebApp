namespace InventoryWebApp.Patterns.Decorator
{
    public class ShippingDecorator : PriceDecorator
    {
        public ShippingDecorator(IPriceComponent component)
            : base(component) { }

        public override decimal GetPrice()
        {
            return _component.GetPrice() + 20; // 20 LYD shipping fee
        }
    }
}
