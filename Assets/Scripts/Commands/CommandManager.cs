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
        private Stack<ICommand> _undoneCommands;

        public int CommandCount => _commands.Count;
        public int UndoneCommandCount => _undoneCommands.Count;

        public CommandManager()
        {
            _commands = new Stack<ICommand>();
            _undoneCommands = new Stack<ICommand>();
        }

        public void ExecuteCommand(ICommand command)
        {
            ExecuteCommand(command, true);
        }

        private void ExecuteCommand(ICommand command, bool clearUndoneCommands)
        {
            if (command == null) return;
            if (clearUndoneCommands && _undoneCommands.Count > 0) _undoneCommands.Clear();

            command.Execute();
            _commands.Push(command);
        }

        public void UndoCommand()
        {
            ICommand latestCommand = _commands.Pop();
            latestCommand.Undo();
            _undoneCommands.Push(latestCommand);
        }

        public void RedoCommand()
        {
            ICommand latestCommand = _undoneCommands.Pop();
            ExecuteCommand(latestCommand, false);
        }
    }

}
