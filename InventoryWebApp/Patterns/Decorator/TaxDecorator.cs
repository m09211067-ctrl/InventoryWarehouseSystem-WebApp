namespace InventoryWebApp.Patterns.Decorator
{
    public class TaxDecorator : PriceDecorator
    {
        public TaxDecorator(IPriceComponent component)
            : base(component) { }

        public override decimal GetPrice()
        {
            return _component.GetPrice() * 1.10m; // 10% tax
        }
    }
}
