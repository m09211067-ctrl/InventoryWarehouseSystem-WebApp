namespace InventoryWebApp.Patterns.Singleton
{
    public class LogManager
    {
        private static LogManager _instance;

        private LogManager() { }

        public static LogManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LogManager();

                return _instance;
            }
        }

        public void Write(string message)
        {
            Console.WriteLine($"[LOG] {DateTime.Now} - {message}");
        }
    }
}
