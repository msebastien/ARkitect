using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

using ARKitect.UI;
using ARKitect.Input;

namespace ARKitect.Core
{
    [RequireComponent(typeof(PinchGestureHandler))]
    [AddComponentMenu("ARkitect/Camera Controller")]
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        [Tooltip("Root object of the scene")]
        private Transform _sceneParent;
        [SerializeField]
        [Tooltip("Defines the 3D scene point around which the camera will move and rotate")]
        private Transform _target;
        public Transform Target
        {
            get
            {
                if (_target != null)
                {
                    return _target;
                }
                else if (_defaultTarget != null)
                {
                    return _defaultTarget;
                }
                else
                {
                    var newTarget = new GameObject("Camera Controller Target");
                    if (_sceneParent != null) newTarget.transform.parent = _sceneParent;
                    _targetPos = newTarget.transform.position;
                    _target = newTarget.transform;
                    _defaultTarget = newTarget.transform;

                    return newTarget.transform;
                }
            }

            set
            {
                _target = value;
                if (_target != null)
                {
                    _targetPos = _target.position;
                }
                else if (_defaultTarget != null)
                {
                    _targetPos = _defaultTarget.position;
                }
                else
                {
                    var newTarget = new GameObject("Camera Controller Target");
                    if (_sceneParent != null) newTarget.transform.parent = _sceneParent;
                    _targetPos = newTarget.transform.position;
                    _target = newTarget.transform;
                    _defaultTarget = newTarget.transform;
                }
            }
        }
        [SerializeField]
        [Tooltip("Defines the 3D scene point around which the camera will move and rotate by default")]
        private Transform _defaultTarget;
        private Vector3 _targetPos = Vector3.zero;
        public Vector3 TargetPosition
        {
            get
            {
                if (_target != null)
                {
                    _targetPos = _target.position;
                }
                else if (_defaultTarget != null)
                {
                    _targetPos = _defaultTarget.position;
                }
                else
                {
                    var newTarget = new GameObject("Camera Controller Target");
                    if (_sceneParent != null) newTarget.transform.parent = _sceneParent;
                    _targetPos = newTarget.transform.position;
                    _target = newTarget.transform;
                    _defaultTarget = newTarget.transform;
                }

                return _targetPos;
            }

            set
            {
                _targetPos = value;
                if (_target != null)
                {
                    _target.transform.position = _targetPos;
                }
                else if (_defaultTarget != null)
                {
                    _target = _defaultTarget.transform;
                    _target.transform.position = _targetPos;
                }
                else
                {
                    var newTarget = new GameObject("Camera Controller Target");
                    newTarget.transform.position = _targetPos;
                    if (_sceneParent != null) newTarget.transform.parent = _sceneParent;
                    _target = newTarget.transform;
                    _defaultTarget = newTarget.transform;
                }
            }
        }

        [Header("Config")]
        [SerializeField]
        private bool _enableCameraControls = true;
        public bool EnableCameraControls
        {
            get { return _enableCameraControls; }
            set { _enableCameraControls = value; }
        }
        [SerializeField]
        private InputActionAsset _inputActionAsset;
        [SerializeField]
        private bool _specifyCustomInitialTargetDistance = false;
        [SerializeField]
        [ShowIf("_specifyCustomInitialTargetDistance")]
        [Tooltip("Initial distance between camera and target")]
        private float _initialTargetDistance = 0.0f;
        [SerializeField]
        private bool _specifyCustomInitialRotation = false;
        [SerializeField]
        [ShowIf("_specifyCustomInitialRotation")]
        private Vector3 _customStartRotation = Vector3.zero;

        [Header("Rotation")]
        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("How sensitive the mouse drag to camera rotation")]
        private float _mouseRotateSpeed = 0.7f;
        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("How sensitive the touch drag to camera rotation")]
        private float _touchRotateSpeed = 0.7f;
        [SerializeField]
        [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
        private float _slerpValue = 0.25f;
        [SerializeField]
        [Range(-180f, 180f)]
        private float _minXRotAngle = -90; // min angle around x axis
        [SerializeField]
        [Range(-180f, 180f)]
        private float _maxXRotAngle = 90; // max angle around x axis

        // Camera state (position, rotation, distance relative to target)
        private float _distanceBetweenCameraAndTarget;
        private Quaternion _cameraRotation; // store the quaternion after the slerp operation
        private Vector3 _cameraPos;

        // Input data
        private InputAction _rotateAction;
        private Vector2 _inputDelta = Vector2.zero;
        private Vector2 _inputRotation = Vector2.zero;

        [Header("Zoom")]
        [SerializeField]
        [Tooltip("Specifies whether the camera's FOV should be changed or the camera should move physically back and forth")]
        private bool _moveCameraBackAndForth = false;
        [SerializeField]
        [Range(0.01f, 1f)]
        private float _mouseZoomSpeed = 0.05f;
        [SerializeField]
        [Range(0.01f, 1f)]
        private float _touchZoomSpeed = 0.03f;
        [SerializeField]
        [HideIf("_moveCameraBackAndForth")]
        [Range(0.0f, 180f)]
        private float _minFOV = 20.0f;
        [SerializeField]
        [HideIf("_moveCameraBackAndForth")]
        [Range(0.0f, 180f)]
        private float _maxFOV = 70.0f;
        private float _defaultFOV;
        [SerializeField]
        [ShowIf("_moveCameraBackAndForth")]
        [Range(0.0f, 50f)]
        private float _minTargetDistance = 2.0f;
        [SerializeField]
        [ShowIf("_moveCameraBackAndForth")]
        [Range(0.0f, 50f)]
        private float _maxTargetDistance = 30.0f;
        private float _defaultTargetDistance;

        // Zoom using mouse scroll wheel
        private InputAction _mouseScrollAction;

        // Zoom using Pinch gesture
        private PinchGestureHandler _pinchGestureHandler;

        private bool _moveCamera = false;

        private UIDetectBackgroundClick _uiDetectBackgroundClick;


        private void Awake()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            if (_pinchGestureHandler == null) _pinchGestureHandler = GetComponent<PinchGestureHandler>();

            _uiDetectBackgroundClick = UIDetectBackgroundClick.Instance;
        }

        private void Start()
        {
            _distanceBetweenCameraAndTarget = Vector3.Distance(_mainCamera.transform.position, TargetPosition);
            _defaultFOV = _mainCamera.fieldOfView;

            // Set camera initial rotation
            if (_specifyCustomInitialRotation)
            {
                Quaternion rotation = Quaternion.Euler(_customStartRotation);
                _cameraRotation = rotation;
                _inputRotation.x = _customStartRotation.y;
                _inputRotation.y = _customStartRotation.x;
            }
            else
            {
                _cameraRotation = _mainCamera.transform.rotation;
                _inputRotation.x = _mainCamera.transform.eulerAngles.x;
                _inputRotation.y = _mainCamera.transform.eulerAngles.y;
            }
            _mainCamera.transform.rotation = _cameraRotation;

            // Set camera initial position
            Vector3 targetDirection = new Vector3(0.0f, 0.0f, -_distanceBetweenCameraAndTarget);
            if (_specifyCustomInitialTargetDistance)
            {
                targetDirection = new Vector3(0.0f, 0.0f, -_initialTargetDistance);
                _defaultTargetDistance = _initialTargetDistance;
            }
            else
            {
                _defaultTargetDistance = _distanceBetweenCameraAndTarget;
            }
            _cameraPos = _cameraRotation * targetDirection + TargetPosition;
            _mainCamera.transform.position = _cameraPos;

            // Input Actions
            SetupInputActions();
            ProcessInputActions();

            // UI Events
            _uiDetectBackgroundClick.OnBackgroundPress.AddListener(MoveCamera);
            _uiDetectBackgroundClick.OnBackgroundRelease.AddListener(StopMovingCamera);
        }

        private void OnDestroy()
        {
            _uiDetectBackgroundClick.OnBackgroundPress.RemoveListener(MoveCamera);
            _uiDetectBackgroundClick.OnBackgroundRelease.RemoveListener(StopMovingCamera);
            if (_defaultTarget != null) Destroy(_defaultTarget.gameObject);
        }

        private void MoveCamera()
        {
            _moveCamera = true;
        }

        private void StopMovingCamera()
        {
            _moveCamera = false;
        }

        [PropertySpace]
        [Button("Reset to default target", 30)]
        public void ResetTarget()
        {
            Target = null;
        }

        private void LateUpdate()
        {
            RotationUpdate();
        }

        private void SetupInputActions()
        {
            try
            {
                _rotateAction = _inputActionAsset.FindAction("Camera/Rotate", true);
                _mouseScrollAction = _inputActionAsset.FindAction("Camera/ScrollWheel", true);
            }
            catch (ArgumentException e)
            {
                Logger.LogError($"CameraController: Input Action not found (Exception:{e.Message})");
            }
        }

        private void ProcessInputActions()
        {
            RotateCameraAction();

            ScrollToZoomAction();
            _pinchGestureHandler.Performed += PinchToZoomCamera;
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f)
                angle += 360f;
            if (angle > 360f)
                angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }

        #region Zoom

        private void ScrollToZoomAction()
        {
            _mouseScrollAction.Enable();
            _mouseScrollAction.performed += (ctx) =>
            {
                if (!_moveCameraBackAndForth)
                {
                    if (_distanceBetweenCameraAndTarget != _defaultTargetDistance) _distanceBetweenCameraAndTarget = _defaultTargetDistance;
                    ZoomCamera(ctx.ReadValue<Vector2>().y * _mouseZoomSpeed);
                }
                else
                {
                    if (_mainCamera.fieldOfView != _defaultFOV) _mainCamera.fieldOfView = _defaultFOV;
                    MoveCameraBackAndForth(ctx.ReadValue<Vector2>().y * _mouseZoomSpeed);
                }
            };
        }

        private void PinchToZoomCamera(InputAction.CallbackContext ctx)
        {
            if (!_enableCameraControls || !_moveCamera) return;
            if (_pinchGestureHandler.TouchCount < 2) return;

            if (!_moveCameraBackAndForth)
            {
                if (_distanceBetweenCameraAndTarget != _defaultTargetDistance) _distanceBetweenCameraAndTarget = _defaultTargetDistance;
                ZoomCamera(_pinchGestureHandler.MagnitudeDelta * _touchZoomSpeed);
            }
            else
            {
                if (_mainCamera.fieldOfView != _defaultFOV) _mainCamera.fieldOfView = _defaultFOV;
                MoveCameraBackAndForth(_pinchGestureHandler.MagnitudeDelta * _touchZoomSpeed);
            }
        }

        private void ZoomCamera(float increment) => _mainCamera.fieldOfView = ClampAngle(_mainCamera.fieldOfView + increment, _minFOV, _maxFOV);

        private void MoveCameraBackAndForth(float increment) => _distanceBetweenCameraAndTarget = Mathf.Clamp(_distanceBetweenCameraAndTarget + increment, _minTargetDistance, _maxTargetDistance);

        #endregion

        #region Rotation        

        private void RotateCameraAction()
        {
            _rotateAction.Enable();

            _rotateAction.started += (ctx) =>
            {
                if (!_enableCameraControls || !_moveCamera) return;

                Logger.LogInfo($"Camera Controller: Rotation Began ({ctx.control.device.displayName})");
            };

            _rotateAction.performed += RotateCamera;
        }

        private void RotateCamera(InputAction.CallbackContext ctx)
        {
            if (!_enableCameraControls || !_moveCamera) return;
            if (ctx.control.device is Touchscreen && _pinchGestureHandler.TouchCount > 1) return;

            _inputDelta = _rotateAction.ReadValue<Vector2>();

            if (ctx.control.device is Mouse)
            {
                _inputRotation.x += -(_inputDelta.y * _mouseRotateSpeed); // around X
                _inputRotation.y += _inputDelta.x * _mouseRotateSpeed;
            }
            else if (ctx.control.device is Touchscreen)
            {
                _inputRotation.x += -(_inputDelta.y * _touchRotateSpeed);
                _inputRotation.y += _inputDelta.x * _touchRotateSpeed;
            }

            _inputRotation.x = ClampAngle(_inputRotation.x, _minXRotAngle, _maxXRotAngle);
        }

        private void RotationUpdate()
        {
            // Camera look direction vector towards the target
            Vector3 direction = Vector3.forward * -_distanceBetweenCameraAndTarget;

            // Value equal to the delta change of our input (mouse or touch) position
            Quaternion newQ = Quaternion.Euler(_inputRotation.x, _inputRotation.y, 0);

            _cameraRotation = Quaternion.Slerp(_cameraRotation, newQ, _slerpValue);  // let cameraRotation value gradually reach newQ which corresponds to our touch
            _cameraPos = _cameraRotation * direction + TargetPosition;

            // Note: By using this method, we avoid setting sequentially position and rotation which is bad for performance.
            // See: https://github.com/microsoft/Microsoft.Unity.Analyzers/blob/main/doc/UNT0022.md
            _mainCamera.transform.SetPositionAndRotation(_cameraPos, _cameraRotation);
        }
        #endregion

    }

}
