namespace InventoryWebApp.Models
{
    public class WarehouseStock
    {
        public int StockID { get; set; }
        public int WarehouseID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}
