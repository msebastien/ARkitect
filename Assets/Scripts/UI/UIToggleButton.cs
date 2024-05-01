using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using Sirenix.OdinInspector;
using TMPro;

using ARKitect.UI.Colors;
using Logger = ARKitect.Core.Logger;

namespace ARKitect.UI
{
    [AddComponentMenu("ARkitect/UI/Toggle Button")]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class UIToggleButton : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField]
        private Button _button;

        [Header("Config")]
        [SerializeField]
        private bool _enableToggleAppearance = true;
        public bool ToggleAppearanceEnabled => _enableToggleAppearance;

        [SerializeField]
        private bool _enableToggleEvent = false;
        public bool ToggleEventEnabled => _enableToggleEvent;

        [SerializeField]
        private TextMeshProUGUI _textComponent;

        [SerializeField]
        private bool _useColorScriptableObject;

        [Header("Button state: Default")]
        [SerializeField]
        private bool _overrideDefaultColor = false;

        [ShowIf("@_overrideDefaultColor && _useColorScriptableObject")]
        [SerializeField]
        private UIColor _defaultColorScriptableObject;

        [ShowIf("@_overrideDefaultColor && !_useColorScriptableObject")]
        [SerializeField]
        private Color _defaultColor = UIColors.Blue;

        [SerializeField]
        private bool _overrideDefaultText = false;

        [ShowIf("_overrideDefaultText")]
        [SerializeField]
        private string _defaultText;

        [SerializeField]
        private ButtonClickedEvent _defaultButtonEvent;
        public ButtonClickedEvent ButtonDefaultEvent
        {
            get => _defaultButtonEvent;
            set => _defaultButtonEvent = value;
        }

        [ShowIf("@_enableToggleAppearance && _useColorScriptableObject")]
        [Header("Button state: Toggled")]
        [SerializeField]
        private UIColor _toggleColorScriptableObject;

        [ShowIf("@_enableToggleAppearance && !_useColorScriptableObject")]
        [Header("Button state: Toggled")]
        [SerializeField]
        private Color _toggleColor = UIColors.Red;

        [ShowIf("_enableToggleAppearance")]
        [SerializeField]
        private string _toggleText;

        [ShowIf("_enableToggleEvent")]
        [SerializeField]
        private ButtonClickedEvent _toggledButtonEvent;
        public ButtonClickedEvent ButtonToggledEvent
        {
            get => _toggledButtonEvent;
            set => _toggledButtonEvent = value;
        }

        private bool _toggle = false;
        public bool Toggled => _toggle;


        private void Awake()
        {
            _button = GetComponent<Button>();
            _textComponent = GetComponentInChildren<TextMeshProUGUI>();

            _button.targetGraphic = GetComponent<Image>();

            // Retrieve color scriptable objects, if enabled and assigned
            if (_useColorScriptableObject)
            {
                if (_defaultColorScriptableObject != null)
                    _defaultColor = _defaultColorScriptableObject._color;

                if (_toggleColorScriptableObject != null)
                    _toggleColor = _toggleColorScriptableObject._color;
            }

            // Override button default color
            if (!_overrideDefaultColor)
                _defaultColor = _button.targetGraphic.color;
            else
                _button.targetGraphic.color = _defaultColor;

            // Override button default text
            if (!_overrideDefaultText)
                _defaultText = _textComponent.text;
            else
                _textComponent.text = _defaultText;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Toggle);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Toggle);
        }

        public void Init(bool toggle)
        {
            if (!_enableToggleEvent && !_enableToggleAppearance)
                _toggle = false;
            else
                _toggle = toggle;

            RefreshAppearance();
        }

        public void Reset()
        {
            _toggle = false;
            _button.targetGraphic.color = _defaultColor;
            _textComponent.text = _defaultText;

            RefreshAppearance();
        }

        /// <summary>
        /// Toggle Button Event Handler.
        /// Called by the event Button.onClick().
        /// </summary>
        private void Toggle()
        {
            if (!_enableToggleEvent && !_enableToggleAppearance)
                _toggle = false;        // Set default state if button cannot be toggled
            else
                _toggle = !_toggle;     // Change button state, if toggleable

            RefreshAppearance();
            InvokeEvent();
        }

        private void RefreshAppearance()
        {
            if (!_enableToggleAppearance || !_toggle)
            {
                _button.targetGraphic.color = _defaultColor;
                _textComponent.text = _defaultText;
            }
            else
            {
                _button.targetGraphic.color = _toggleColor;
                _textComponent.text = _toggleText;
            }
        }

        private void InvokeEvent()
        {
            if (!_enableToggleEvent || !_toggle)
                _defaultButtonEvent?.Invoke();
            else
                _toggledButtonEvent?.Invoke();
        }
    }

}
