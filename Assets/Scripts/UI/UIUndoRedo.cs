using UnityEngine;
using UnityEngine.UI;

using ARKitect.Core;

namespace ARKitect.UI
{
    /// <summary>
    /// Manage UI Buttons to undo or redo app commands
    /// </summary>
    [AddComponentMenu("ARKitect/UI/Undo Redo")]
    public class UIUndoRedo : MonoBehaviour
    {
        [SerializeField]
        private Button _undoButton;
        [SerializeField]
        private Button _redoButton;

        private void Awake()
        {
            if (_undoButton == null) _undoButton = GetComponent<Button>();
            if (_redoButton == null) _redoButton = GetComponent<Button>();
        }

        private void Start()
        {
            UpdateButtons();
        }

        private void OnEnable()
        {
            _undoButton.onClick.AddListener(ARKitectApp.CommandManager.UndoCommand);
            _redoButton.onClick.AddListener(ARKitectApp.CommandManager.RedoCommand);

            _undoButton.onClick.AddListener(UpdateButtons);
            _redoButton.onClick.AddListener(UpdateButtons);
        }

        private void OnDisable()
        {
            _undoButton.onClick.RemoveListener(ARKitectApp.CommandManager.UndoCommand);
            _redoButton.onClick.RemoveListener(ARKitectApp.CommandManager.RedoCommand);

            _undoButton.onClick.RemoveListener(UpdateButtons);
            _redoButton.onClick.RemoveListener(UpdateButtons);
        }

        /// <summary>
        /// Make undo/redo buttons interactable or not, depending if there are remaining commands to undo or redo.
        /// </summary>
        private void UpdateButtons()
        {
            _undoButton.interactable = ARKitectApp.CommandManager.CommandCount > 0;
            _redoButton.interactable = ARKitectApp.CommandManager.UndoneCommandCount > 0;
        }
    }

}
