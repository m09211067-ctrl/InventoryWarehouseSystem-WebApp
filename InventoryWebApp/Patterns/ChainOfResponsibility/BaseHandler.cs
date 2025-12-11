using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.ChainOfResponsibility
{
    public abstract class BaseHandler : IHandler
    {
        private IHandler? _next;

        public IHandler SetNext(IHandler handler)
        {
            _next = handler;
            return handler;
        }

        public virtual void Handle(Product product)
        {
            _next?.Handle(product);
        }
    }
}
