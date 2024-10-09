using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARKitect.Commands
{
    /// <summary>
    /// This class stores the history of all commands executed in the app
    /// </summary>
    public class CommandHistory
    {
        private LinkedList<ICommand> _commands = new LinkedList<ICommand>();

        private int _maxSize = 20;
        private int _cancelledCommandCount = 0;

        private LinkedListNode<ICommand> _current; // point to the current active command
        public ICommand Current => _current.Value;
        public int Count => _commands.Count;

        public CommandHistory() { }
        public CommandHistory(int maxSize) => _maxSize = maxSize;
        public CommandHistory(ICommand command) => Add(command);
        public CommandHistory(ICommand command, int maxSize)
        {
            _maxSize = maxSize;
            Add(command);
        }

        public void Add(ICommand command)
        {
            if (_commands.Count >= _maxSize) _commands.RemoveFirst();
            if (_cancelledCommandCount > 0) ClearCancelledCommands();
            _current = _commands.AddLast(command);
        }

        public ICommand Cancel()
        {
            if (_current == null) return null;

            var prev = _current.Previous;
            var cmdToCancel = _current;
            _current = prev;
            _cancelledCommandCount++;
            return cmdToCancel.Value;
        }

        public ICommand Restore()
        {
            if (_current == null) return null;

            var cmdToRestore = _current.Next;
            _current = cmdToRestore;
            _cancelledCommandCount--;
            return cmdToRestore.Value;
        }

        public void ClearCancelledCommands() => RemoveAfter(_current);

        private void RemoveAfter(LinkedListNode<ICommand> node)
        {
            if (node == null) return;

            // First node after the current one
            var current = node.Next;

            // Remove nodes
            while (!current.Equals(_commands.Last))
            {
                var next = current.Next;
                _commands.Remove(current);
                current = next;
            }
            _commands.RemoveLast();
        }

    }

}
