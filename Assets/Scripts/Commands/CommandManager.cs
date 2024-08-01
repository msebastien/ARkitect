using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARKitect.Commands
{
    /// <summary>
    /// Invoke commands and store them in a history
    /// </summary>
    public class CommandManager
    {
        private Stack<ICommand> _commands = new Stack<ICommand>();

        public CommandManager()
        {
            _commands = new Stack<ICommand>();
        }

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commands.Push(command);
        }
    }

}
