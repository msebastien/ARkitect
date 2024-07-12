using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

using Logger = ARKitect.Core.Logger;

namespace ARKitect.Input
{
    [AddComponentMenu("ARkitect/Input/Pinch Gesture Handler")]
    public class PinchGestureHandler : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        [Tooltip("Allow to define manually the actions' bindings in the Unity Inspector")]
        private bool _defineActionsManually = false;

        [Header("Action Map")]
        [SerializeField]
        [HideIf("_defineActionsManually")]
        private InputActionAsset _inputActionAsset;

        [Header("Actions")]
        [SerializeField]
        [ShowIf("_defineActionsManually")]
        private InputAction _touch0ContactAction;
        [SerializeField]
        [ShowIf("_defineActionsManually")]
        private InputAction _touch1ContactAction;
        [SerializeField]
        [ShowIf("_defineActionsManually")]
        private InputAction _touch0PosAction;
        [SerializeField]
        [ShowIf("_defineActionsManually")]
        private InputAction _touch1PosAction;

        private int _touchCount = 0;
        public int TouchCount => _touchCount;

        public float Magnitude => (_touch0PosAction.ReadValue<Vector2>() - _touch1PosAction.ReadValue<Vector2>()).magnitude;

        private float _prevMagnitude = 0f;
        public float PrevMagnitude
        {
            get => _prevMagnitude;
            set => _prevMagnitude = value;
        }

        public float MagnitudeDelta
        {
            get
            {
                if (PrevMagnitude == 0) PrevMagnitude = Magnitude;
                float delta = Magnitude - PrevMagnitude;
                PrevMagnitude = Magnitude;

                return delta;
            }
        }

        public event Action<InputAction.CallbackContext> Performed;


        private void Start()
        {
            SetupInputActions();
            ProcessGestureActions();
        }

        private void SetupInputActions()
        {
            if (_inputActionAsset != null)
                LoadActionsFromMap();
            else if (!_defineActionsManually)
                CreateActionMap();
        }

        private void LoadActionsFromMap()
        {
            try
            {
                _touch0ContactAction = _inputActionAsset.FindAction("Gesture/Touch0Contact", true);
                _touch1ContactAction = _inputActionAsset.FindAction("Gesture/Touch1Contact", true);
                _touch0PosAction = _inputActionAsset.FindAction("Gesture/Touch0Pos", true);
                _touch1PosAction = _inputActionAsset.FindAction("Gesture/Touch1Pos", true);
            }
            catch (ArgumentException e)
            {
                Logger.LogError($"CameraController: Input Action not found (Exception:{e.Message})");
            }
        }

        private void CreateActionMap()
        {
            var inputActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
            var actionMap = new InputActionMap("Gesture");

            CreateActions(actionMap);

            inputActionAsset.AddActionMap(actionMap);
        }

        private void CreateActions(InputActionMap map)
        {
            _touch0ContactAction = map.AddAction("Touch0Contact", InputActionType.Button, "<Touchscreen>/touch0/press", null, null, null, "Touch");
            _touch1ContactAction = map.AddAction("Touch1Contact", InputActionType.Button, "<Touchscreen>/touch1/press", null, null, null, "Touch");
            _touch0PosAction = map.AddAction("Touch0Pos", InputActionType.Value, "<Touchscreen>/touch0/position", null, null, null, "Touch");
            _touch1PosAction = map.AddAction("Touch1Pos", InputActionType.Value, "<Touchscreen>/touch1/position", null, null, null, "Touch");
        }

        private void ProcessGestureActions()
        {
            _touch0ContactAction.Enable();
            _touch1ContactAction.Enable();

            _touch0ContactAction.performed += _ => _touchCount++;
            _touch1ContactAction.performed += _ => _touchCount++;
            _touch0ContactAction.canceled += _ =>
            {
                _touchCount--;
                _prevMagnitude = 0;
            };
            _touch1ContactAction.canceled += _ =>
            {
                _touchCount--;
                _prevMagnitude = 0;
            };

            _touch0PosAction.Enable();
            _touch1PosAction.Enable();

            _touch1PosAction.performed += (ctx) => Performed.Invoke(ctx);
        }

    }

}
