using UnityEngine;
using UnityEngine.UI;

using ARKitect.Core;
using ARKitect.Commands;

namespace ARKitect.UI
{
    /// <summary>
    /// Manage UI Buttons to undo or redo app commands
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Undo Redo")]
    public class UIUndoRedo : MonoBehaviour
    {
        [SerializeField]
        private Button _undoButton;
        [SerializeField]
        private Button _redoButton;

        private CommandManager _commandManager;

        private void Awake()
        {
            _commandManager = ARKitectApp.Instance.CommandManager;
        }

        private void Start()
        {
            UpdateButtons();
        }

        private void OnEnable()
        {
            _undoButton.onClick.AddListener(_commandManager.UndoCommand);
            _redoButton.onClick.AddListener(_commandManager.RedoCommand);

            _undoButton.onClick.AddListener(UpdateButtons);
            _redoButton.onClick.AddListener(UpdateButtons);

            _commandManager.OnExecuteCommand.AddListener(ExecuteOrUndoCommand);
            _commandManager.OnUndoCommand.AddListener(ExecuteOrUndoCommand);
        }

        private void OnDisable()
        {
            _undoButton.onClick.RemoveListener(_commandManager.UndoCommand);
            _redoButton.onClick.RemoveListener(_commandManager.RedoCommand);

            _undoButton.onClick.RemoveListener(UpdateButtons);
            _redoButton.onClick.RemoveListener(UpdateButtons);

            _commandManager.OnExecuteCommand.RemoveListener(ExecuteOrUndoCommand);
            _commandManager.OnUndoCommand.RemoveListener(ExecuteOrUndoCommand);
        }

        /// <summary>
        /// Update button states when executing or undoing a command
        /// </summary>
        /// <param name="command"></param>
        private void ExecuteOrUndoCommand(ICommand command) => UpdateButtons();

        /// <summary>
        /// Make undo/redo buttons interactable or not, depending if there are remaining commands to undo or redo.
        /// </summary>
        private void UpdateButtons()
        {
            _undoButton.interactable = _commandManager.ActiveCommandCount > 0;
            _redoButton.interactable = _commandManager.UndoneCommandCount > 0;
        }

    }

}
