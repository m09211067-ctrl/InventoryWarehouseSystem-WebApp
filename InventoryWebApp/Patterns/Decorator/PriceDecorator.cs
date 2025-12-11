namespace InventoryWebApp.Patterns.Decorator
{
    public abstract class PriceDecorator : IPriceComponent
    {
        protected readonly IPriceComponent _component;

        public PriceDecorator(IPriceComponent component)
        {
            _component = component;
        }

        public abstract decimal GetPrice();
    }
}
