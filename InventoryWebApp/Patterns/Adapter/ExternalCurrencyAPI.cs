namespace InventoryWebApp.Patterns.Adapter
{
    // خدمة خارجية 
    public class ExternalCurrencyAPI
    {
        public decimal FetchUsdToLydRate()
        {
            // الرقم مثال فقط
            return 5.10m; // 1 USD = 5.10 LYD
        }
    }
}
