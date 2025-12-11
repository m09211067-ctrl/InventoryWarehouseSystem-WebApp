namespace InventoryWebApp.Patterns.Command
{
    public interface ICommand
    {
        void Execute();
        void Undo();  // نضيفها إذا أردنا دعم الرجوع لاحقًا
    }
}
