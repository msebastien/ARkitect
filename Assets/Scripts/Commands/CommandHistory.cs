using System.Collections;
using System.Collections.Generic;

namespace ARKitect.Commands
{
    /// <summary>
    /// This class stores the history of all commands executed in the app
    /// </summary>
    public class CommandHistory
    {
        private LinkedList<ICommand> _commands = new LinkedList<ICommand>();

        private LinkedListNode<ICommand> _current; // point to the current active command

        private int _maxSize = 20;
        private int _cancelledCommandCount = 0;

        /// <summary>
        /// Current active command / Last executed command
        /// </summary>
        public ICommand Current => _current.Value;

        /// <summary>
        /// Total number of executed commands
        /// </summary>
        public int Count => _commands.Count;

        /// <summary>
        /// Number of cancelled commands
        /// </summary>
        public int CancelledCount => _cancelledCommandCount;

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
            if (Count - CancelledCount <= 0) return null;

            var prev = _current.Previous;
            var cmdToCancel = _current;
            _current = prev;
            _cancelledCommandCount++;
            return cmdToCancel.Value;
        }

        public ICommand Restore()
        {
            if (CancelledCount <= 0) return null;

            LinkedListNode<ICommand> cmdToRestore;
            if (_current == null)
                cmdToRestore = _commands.First;
            else
                cmdToRestore = _current.Next;

            _current = cmdToRestore;
            _cancelledCommandCount--;
            return cmdToRestore.Value;
        }

        public void ClearCancelledCommands()
        {
            if(_current != null)
                RemoveAfter(_current);
            else
                _commands.Clear();
            _cancelledCommandCount = 0;
        }

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
