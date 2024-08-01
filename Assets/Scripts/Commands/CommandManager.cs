using System.Collections;
using System.Collections.Generic;

namespace ARKitect.Commands
{
    /// <summary>
    /// Invoke commands and store them in a history
    /// </summary>
    public class CommandManager
    {
        private Stack<ICommand> _commands;

        public CommandManager()
        {
            _commands = new Stack<ICommand>();
        }

        public void ExecuteCommand(ICommand command)
        {
            if(command == null) return;

            command.Execute();
            _commands.Push(command);
        }

        public void UndoCommand()
        {
            ICommand latestCommand = _commands.Pop();
            latestCommand.Undo();
        }
    }

}
