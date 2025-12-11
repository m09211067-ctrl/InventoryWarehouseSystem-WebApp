namespace InventoryWebApp.Models
{
    public class Warehouse
    {
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public string BranchName { get; set; }
        public int Capacity { get; set; }
        public int CurrentLoad { get; set; }
    }
}
