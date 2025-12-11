namespace InventoryWebApp.Patterns.Adapter
{
    public class CurrencyAdapter : ICurrencyService
    {
        private readonly ExternalCurrencyAPI _externalApi;

        public CurrencyAdapter(ExternalCurrencyAPI api)
        {
            _externalApi = api;
        }

        public decimal GetExchangeRate()
        {
            return _externalApi.FetchUsdToLydRate();
        }
    }
}
