using UnityEngine;

namespace ARKitect.UI.Items
{
    /// <summary>
    /// Allows to select and execute an action for a specific slot and its item.
    /// Needs to have a UISlotActions component in a parent GameObject
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Slot Action Handler")]
    [RequireComponent(typeof(UIToggleButton))]
    public class UISlotActionHandler : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        [Tooltip("Enable the automatic subscribe to the OnClick() event of a button")]
        private bool _autoSubscribeToToggleButtonClickEvent = true;

        [Header("Button: Selected Slot Action")]
        [SerializeField]
        private SlotActionType _action;

        [Header("Actions")]
        [SerializeField]
        private UISlotActions _slotActions;

        private UIToggleButton _toggleButton;

        private void Awake()
        {
            if (_slotActions == null)
                _slotActions = GetComponentInParent<UISlotActions>(true);

            _toggleButton = GetComponent<UIToggleButton>();
        }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            if (_toggleButton != null)
            {
                var initialActionState = _slotActions.GetActionState(_action);
                _toggleButton.SetToggle(initialActionState);
            }
        }

        private void OnEnable()
        {
            if (_toggleButton != null && _autoSubscribeToToggleButtonClickEvent)
                _toggleButton.ButtonDefaultEvent.AddListener(ExecuteAction);
        }

        private void OnDisable()
        {
            if (_toggleButton != null && _autoSubscribeToToggleButtonClickEvent)
                _toggleButton.ButtonDefaultEvent.RemoveListener(ExecuteAction);
        }

        public void ExecuteAction()
        {
            _slotActions.Execute(_action, _toggleButton.Toggled);
        }

    }

}