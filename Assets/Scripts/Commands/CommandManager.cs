using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ARKitect.Events;

namespace ARKitect.Events
{
    [Serializable]
    public class CommandEvent : UnityEvent<Commands.ICommand> { }
}

namespace ARKitect.Commands
{
    /// <summary>
    /// Execute and undo commands
    /// </summary>
    [AddComponentMenu("ARkitect/Command Manager")]
    public class CommandManager : MonoBehaviour
    {
        private Stack<ICommand> _commands;
        private Stack<ICommand> _undoneCommands;

        public int CommandCount => _commands.Count;
        public int UndoneCommandCount => _undoneCommands.Count;

        public CommandEvent OnExecuteCommand;
        public CommandEvent OnUndoCommand;

        public void Awake()
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
            OnExecuteCommand.Invoke(command);
        }

        public void UndoCommand()
        {
            ICommand latestCommand = _commands.Pop();
            latestCommand.Undo();
            _undoneCommands.Push(latestCommand);
            OnUndoCommand.Invoke(latestCommand);
        }

        public void RedoCommand()
        {
            ICommand latestCommand = _undoneCommands.Pop();
            ExecuteCommand(latestCommand, false);
        }
    }

}
