namespace InventoryWebApp.Models
{
    public class StockMovement
    {
        public int MovementID { get; set; }      // PRIMARY KEY
        public int ProductID { get; set; }       // FK -> Products
        public int WarehouseID { get; set; }     // FK -> Warehouses
        public string MovementType { get; set; } = "";   // IN / OUT / TRANSFER
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
    }
}
