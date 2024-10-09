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
        private CommandHistory _commands;

        private int _maxHistorySize = 20;

        /// <summary>
        /// Total number of executed commands
        /// </summary>
        public int CommandCount => _commands.Count;

        /// <summary>
        /// Number of undone commands
        /// </summary>
        public int UndoneCommandCount => _commands.CancelledCount;

        public CommandEvent OnExecuteCommand;
        public CommandEvent OnUndoCommand;

        public void Awake()
        {
            _commands = new CommandHistory(_maxHistorySize);
        }

        public void ExecuteCommand(ICommand command)
        {
            if (command == null) return;

            command.Execute();
            _commands.Add(command);
            OnExecuteCommand.Invoke(command);
        }

        public void UndoCommand()
        {
            if (CommandCount - UndoneCommandCount <= 0) return;

            ICommand cancelledCommand = _commands.Cancel();
            if (cancelledCommand != null)
            {
                cancelledCommand.Undo();
                OnUndoCommand.Invoke(cancelledCommand);
            }

        }

        public void RedoCommand()
        {
            if (UndoneCommandCount <= 0) return;

            ICommand restoredCommand = _commands.Restore();
            if (restoredCommand != null)
            {
                restoredCommand.Execute();
                OnExecuteCommand.Invoke(restoredCommand);
            }
        }

    }

}
