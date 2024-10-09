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

        private ARKitectApp _app;

        private void Awake()
        {
            _app = ARKitectApp.Instance;
        }

        private void Start()
        {
            UpdateButtons();
        }

        private void OnEnable()
        {
            _undoButton.onClick.AddListener(_app.CommandManager.UndoCommand);
            _redoButton.onClick.AddListener(_app.CommandManager.RedoCommand);

            _undoButton.onClick.AddListener(UpdateButtons);
            _redoButton.onClick.AddListener(UpdateButtons);

            _app.CommandManager.OnExecuteCommand.AddListener(ExecuteOrUndoCommand);
            _app.CommandManager.OnUndoCommand.AddListener(ExecuteOrUndoCommand);
        }

        private void OnDisable()
        {
            _undoButton.onClick.RemoveListener(_app.CommandManager.UndoCommand);
            _redoButton.onClick.RemoveListener(_app.CommandManager.RedoCommand);

            _undoButton.onClick.RemoveListener(UpdateButtons);
            _redoButton.onClick.RemoveListener(UpdateButtons);

            _app.CommandManager.OnExecuteCommand.RemoveListener(ExecuteOrUndoCommand);
            _app.CommandManager.OnUndoCommand.RemoveListener(ExecuteOrUndoCommand);
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
            _undoButton.interactable = _app.CommandManager.CommandCount > 0;
            _redoButton.interactable = _app.CommandManager.UndoneCommandCount > 0;
        }

    }

}
