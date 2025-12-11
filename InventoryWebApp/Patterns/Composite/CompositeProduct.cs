namespace InventoryWebApp.Patterns.Composite
{
    public class CompositeProduct : IProductComponent
    {
        private readonly List<IProductComponent> _components = new();
        public string Name { get; set; }

        public CompositeProduct(string name)
        {
            Name = name;
        }

        public void Add(IProductComponent component)
        {
            _components.Add(component);
        }

        public void Remove(IProductComponent component)
        {
            _components.Remove(component);
        }

        public string GetName() => Name;

        public decimal GetPrice()
        {
            decimal total = 0;
            foreach (var c in _components)
                total += c.GetPrice();

            return total;
        }
    }
}
