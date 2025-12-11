using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.ChainOfResponsibility
{
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);
        void Handle(Product product);
    }
}
