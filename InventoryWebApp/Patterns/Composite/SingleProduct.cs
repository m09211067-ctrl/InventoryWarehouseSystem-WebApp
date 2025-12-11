namespace InventoryWebApp.Patterns.Composite
{
    public class SingleProduct : IProductComponent
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public SingleProduct(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string GetName() => Name;

        public decimal GetPrice() => Price;
    }
}
