using System.Collections.Generic;

namespace InventoryWebApp.Patterns.Command
{
    public class CommandInvoker
    {
        private readonly Stack<ICommand> _history = new Stack<ICommand>();

        // تنفيذ الأمر
        public void Execute(ICommand command)
        {
            command.Execute();
            _history.Push(command);
        }

        // التراجع عن آخر أمر
        public void UndoLast()
        {
            if (_history.Count > 0)
            {
                var lastCommand = _history.Pop();
                lastCommand.Undo();
            }
        }
    }
}
