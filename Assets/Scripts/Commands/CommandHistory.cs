using System.Collections;
using System.Collections.Generic;

namespace ARKitect.Commands
{
    /// <summary>
    /// This class stores the history of all commands executed in the app.
    /// </summary>
    public class CommandHistory
    {
        private LinkedList<ICommand> _commands = new LinkedList<ICommand>();

        private LinkedListNode<ICommand> _current; // point to the current active command (last executed command)

        private int _maxSize = 20;
        private int _cancelledCommandCount = 0;

        public bool IsEmpty => _commands.Count == 0;

        /// <summary>
        /// Total number of commands in the history (active + cancelled).
        /// </summary>
        public int Count => _commands.Count;

        /// <summary>
        /// Number of cancelled commands.
        /// </summary>
        public int CancelledCount => _cancelledCommandCount;

        /// <summary>
        /// Number of currently active executed commands that can be cancelled.
        /// </summary>
        public int ActiveCount => _commands.Count - _cancelledCommandCount;

        public CommandHistory() { }
        public CommandHistory(int maxSize) => _maxSize = maxSize;
        public CommandHistory(ICommand command) => Add(command);
        public CommandHistory(ICommand command, int maxSize)
        {
            _maxSize = maxSize;
            Add(command);
        }

        /// <summary>
        /// Add an active/executed command to the end of the history.
        /// Cancelled commands remaining in the history are cleared.
        /// If the history reaches its maximum size, the first command of the history will be removed.
        /// </summary>
        /// <param name="command">Active/Executed Command by the Command Manager.</param>
        public void Add(ICommand command)
        {
            if (_commands.Count >= _maxSize) _commands.RemoveFirst();
            if (_cancelledCommandCount > 0) ClearCancelledCommands();
            _current = _commands.AddLast(command);
        }

        /// <summary>
        /// Clear completely the history
        /// </summary>
        public void Clear()
        {
            _current = null;
            _commands.Clear();
            _cancelledCommandCount = 0;
        }

        /// <summary>
        /// Return the current active command to cancel/unexecute.
        /// </summary>
        /// <returns>Active command to cancel/unexecute if there is one, else null.</returns>
        public ICommand Cancel()
        {
            if (ActiveCount <= 0) return null; // If there is no active commands (all have been cancelled), do nothing

            var cmdToCancel = _current;     // The command to cancel is the current active one

            LinkedListNode<ICommand> prev;
            if (_current.Previous == null)  // We are on the first node
                prev = _commands.First;     // Keep the pointer to the first node, as its "previous" pointer is obviously null
            else
                prev = _current.Previous;   // Else, just keep going normally to the previous node          
            _current = prev;                // Then, go the previous active one

            _cancelledCommandCount++;       // Increment the number of cancelled command

            return cmdToCancel.Value;       // Return the command to cancel (Command Manager calls Undo() on this command)
        }

        /// <summary>
        /// Return the cancelled command to restore.
        /// </summary>
        /// <returns>Cancelled command to restore if there is one, else null.</returns>
        public ICommand Restore()
        {
            if (CancelledCount <= 0) return null; // If there is no cancelled commands (all have been restored), do nothing

            LinkedListNode<ICommand> cmdToRestore;
            if (ActiveCount == 0 && _current == _commands.First)    // We are on the first node with no active command
                cmdToRestore = _current;                            // Restore the current one before going to next one
            else
                cmdToRestore = _current.Next;                       // Else, just keep going normally to the next node
            _current = cmdToRestore;                                // Make command to restore, the current node

            _cancelledCommandCount--;                               // Decrement the number of cancelled command

            return cmdToRestore.Value;                              // Return the command to restore (Command Manager calls Execute() on this command)
        }

        /// <summary>
        /// Remove all cancelled commands in the history.
        /// </summary>
        private void ClearCancelledCommands()
        {
            if (ActiveCount == 0 && _current == _commands.First)    // We are on the first node with no active command
                Clear();                                            // Clear all the history
            else
                RemoveAfter(_current);                              // Else, clear the nodes (cancelled commands) following the current active one
            _cancelledCommandCount = 0;                             // Set the number of cancelled commands to 0 as all have been cleared
        }

        /// <summary>
        /// Remove all nodes after the specified one.
        /// The specified node and all the previous ones are preserved.
        /// </summary>
        /// <param name="node">The node to start from.</param>
        private void RemoveAfter(LinkedListNode<ICommand> node)
        {
            if (node == null) return;

            // First node after the current one
            var current = node.Next;

            // Remove nodes
            while (current != _commands.Last)
            {
                var next = current.Next;
                _commands.Remove(current);
                current = next;
            }
            _commands.RemoveLast();
        }

    }

}
