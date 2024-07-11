using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

using ARKitect.UI;

namespace ARKitect.Core
{
    [AddComponentMenu("ARkitect/Camera Controller")]
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        [Tooltip("Defines the 3D scene point around which the camera will move and rotate")]
        private Transform target;

        [Header("Config")]
        [SerializeField]
        private bool enableCameraControls = true;
        [SerializeField]
        private InputActionAsset actionMap;
        [SerializeField]
        private bool specifyCustomInitialTargetDistance = false;
        [SerializeField]
        [ShowIf("specifyCustomInitialTargetDistance")]
        [Tooltip("Initial distance between camera and target")]
        private float initialTargetDistance = 0.0f;
        [SerializeField]
        private bool specifyCustomInitialRotation = false;
        [SerializeField]
        [ShowIf("specifyCustomInitialRotation")]
        private Vector3 customStartRotation = Vector3.zero;
        public bool EnableCameraControls
        {
            get { return enableCameraControls; }
            set { enableCameraControls = value; }
        }

        [Header("Rotation")]
        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("How sensitive the mouse drag to camera rotation")]
        private float mouseRotateSpeed = 0.7f;
        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("How sensitive the touch drag to camera rotation")]
        private float touchRotateSpeed = 0.7f;
        [SerializeField]
        [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
        private float slerpValue = 0.25f;
        [SerializeField]
        [Range(-180f, 180f)]
        private float minXRotAngle = -90; // min angle around x axis
        [SerializeField]
        [Range(-180f, 180f)]
        private float maxXRotAngle = 90; // max angle around x axis

        // Camera state (position, rotation, distance relative to target)
        private float distanceBetweenCameraAndTarget;
        private Quaternion cameraRotation; // store the quaternion after the slerp operation
        private Vector3 cameraPos;

        // Input data
        private InputAction rotateAction;
        private Vector2 inputDelta = Vector2.zero;
        private Vector2 inputRotation = Vector2.zero;

        [Header("Zoom")]
        [SerializeField]
        [Tooltip("Specifies whether the camera's FOV should be changed or the camera should move physically back and forth")]
        private bool moveCameraBackAndForth = false;
        [SerializeField]
        private float mouseZoomSpeed = 0.05f;
        [SerializeField]
        private float touchZoomSpeed = 0.05f;
        [SerializeField]
        [HideIf("moveCameraBackAndForth")]
        [Range(0.0f, 180f)]
        private float minFOV = 20.0f;
        [SerializeField]
        [HideIf("moveCameraBackAndForth")]
        [Range(0.0f, 180f)]
        private float maxFOV = 70.0f;
        private float defaultFOV;
        [SerializeField]
        [ShowIf("moveCameraBackAndForth")]
        [Range(0.0f, 50f)]
        private float minTargetDistance = 0.5f;
        [SerializeField]
        [ShowIf("moveCameraBackAndForth")]
        [Range(0.0f, 50f)]
        private float maxTargetDistance = 30.0f;


        // Zoom using mouse scroll wheel
        private InputAction mouseScrollAction;

        // Zoom using Pinch gesture
        private InputAction touch0ContactAction;
        private InputAction touch1ContactAction;
        private InputAction touch0PosAction;
        private InputAction touch1PosAction;
        private float prevMagnitude = 0;
        private int touchCount = 0;

        private bool moveCamera = false;

        private UIDetectBackgroundClick uiDetectBackgroundClick;


        private void Awake()
        {
            if (mainCamera == null) mainCamera = Camera.main;

            uiDetectBackgroundClick = UIDetectBackgroundClick.Instance;
        }

        private void Start()
        {
            distanceBetweenCameraAndTarget = Vector3.Distance(mainCamera.transform.position, target.position);
            defaultFOV = mainCamera.fieldOfView;

            // Set camera initial rotation
            if (specifyCustomInitialRotation)
            {
                Quaternion rotation = Quaternion.Euler(customStartRotation);
                cameraRotation = rotation;
                inputRotation.x = customStartRotation.y;
                inputRotation.y = customStartRotation.x;
            }
            else
            {
                cameraRotation = mainCamera.transform.rotation;
                inputRotation.x = mainCamera.transform.eulerAngles.x;
                inputRotation.y = mainCamera.transform.eulerAngles.y;
            }
            mainCamera.transform.rotation = cameraRotation;

            // Set camera initial position
            Vector3 targetDirection = new Vector3(0.0f, 0.0f, -distanceBetweenCameraAndTarget);

            if (specifyCustomInitialTargetDistance)
                targetDirection = new Vector3(0.0f, 0.0f, -initialTargetDistance);

            cameraPos = cameraRotation * targetDirection + target.position;
            mainCamera.transform.position = cameraPos;

            // Input Actions
            SetupInputActions();
            ProcessInputActions();

            // UI Events
            uiDetectBackgroundClick.OnBackgroundPress.AddListener(MoveCamera);
            uiDetectBackgroundClick.OnBackgroundRelease.AddListener(StopMovingCamera);
        }

        private void OnDestroy()
        {
            uiDetectBackgroundClick.OnBackgroundPress.RemoveListener(MoveCamera);
            uiDetectBackgroundClick.OnBackgroundRelease.RemoveListener(StopMovingCamera);
        }

        private void MoveCamera()
        {
            moveCamera = true;
        }

        private void StopMovingCamera()
        {
            moveCamera = false;
        }

        private void LateUpdate()
        {
            RotationUpdate();
        }

        private void SetupInputActions()
        {
            try
            {
                rotateAction = actionMap.FindAction("Camera/Rotate", true);

                mouseScrollAction = actionMap.FindAction("Camera/ScrollWheel", true);
                touch0ContactAction = actionMap.FindAction("Camera/Touch0Contact", true);
                touch1ContactAction = actionMap.FindAction("Camera/Touch1Contact", true);
                touch0PosAction = actionMap.FindAction("Camera/Touch0Pos", true);
                touch1PosAction = actionMap.FindAction("Camera/Touch1Pos", true);
            }
            catch (ArgumentException e)
            {
                Logger.LogError($"CameraController: Input Action not found (Exception:{e.Message})");
            }
        }

        private void ProcessInputActions()
        {
            RotateCameraAction();

            ScrollToZoomActions();
            PinchToZoomGestureActions();
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }

        #region Zoom

        private void ScrollToZoomActions()
        {
            mouseScrollAction.Enable();
            mouseScrollAction.performed += ctx =>
            {
                if (!moveCameraBackAndForth)
                {
                    ZoomCamera(ctx.ReadValue<Vector2>().y * mouseZoomSpeed);
                }
                else
                {
                    if (mainCamera.fieldOfView != defaultFOV) mainCamera.fieldOfView = defaultFOV;
                    MoveCameraBackAndForth(ctx.ReadValue<Vector2>().y * mouseZoomSpeed);
                }
            };
        }

        private void PinchToZoomGestureActions()
        {
            touch0ContactAction.Enable();
            touch1ContactAction.Enable();

            touch0ContactAction.performed += _ => touchCount++;
            touch1ContactAction.performed += _ => touchCount++;
            touch0ContactAction.canceled += _ =>
            {
                touchCount--;
                prevMagnitude = 0;
            };
            touch1ContactAction.canceled += _ =>
            {
                touchCount--;
                prevMagnitude = 0;
            };

            touch0PosAction.Enable();
            touch1PosAction.Enable();
            touch1PosAction.performed += PinchToZoomCamera;
        }

        private void PinchToZoomCamera(InputAction.CallbackContext ctx)
        {
            if (!enableCameraControls || !moveCamera) return;
            if (touchCount < 2) return;

            var magnitude = (touch0PosAction.ReadValue<Vector2>() - touch1PosAction.ReadValue<Vector2>()).magnitude;

            if (prevMagnitude == 0) prevMagnitude = magnitude;

            var difference = magnitude - prevMagnitude;
            prevMagnitude = magnitude;

            if (!moveCameraBackAndForth)
            {
                ZoomCamera(difference * touchZoomSpeed);
            }
            else
            {
                if (mainCamera.fieldOfView != defaultFOV) mainCamera.fieldOfView = defaultFOV;
                MoveCameraBackAndForth(difference * touchZoomSpeed);
            }

        }

        private void ZoomCamera(float increment) => mainCamera.fieldOfView = ClampAngle(mainCamera.fieldOfView + increment, minFOV, maxFOV);

        private void MoveCameraBackAndForth(float increment) => distanceBetweenCameraAndTarget = Mathf.Clamp(distanceBetweenCameraAndTarget + increment, minTargetDistance, maxTargetDistance);

        #endregion

        #region Rotation        

        private void RotateCameraAction()
        {
            rotateAction.Enable();

            rotateAction.started += (ctx) =>
            {
                if (!enableCameraControls || !moveCamera) return;

                Logger.LogInfo($"Camera Controller: Rotation Began ({ctx.control.device.displayName})");
            };

            rotateAction.performed += RotateCamera;
        }

        private void RotateCamera(InputAction.CallbackContext ctx)
        {
            if (!enableCameraControls || !moveCamera) return;

            inputDelta = rotateAction.ReadValue<Vector2>();
            if (ctx.control.device is Mouse)
            {
                inputRotation.x += -inputDelta.y * mouseRotateSpeed; // around X
                inputRotation.y += inputDelta.x * mouseRotateSpeed;

                inputRotation.x = ClampAngle(inputRotation.x, minXRotAngle, maxXRotAngle);
            }

            else if (ctx.control.device is Touchscreen)
            {
                if (touchCount > 1) return;

                inputRotation.x += inputDelta.y * touchRotateSpeed;
                inputRotation.y += -inputDelta.x * touchRotateSpeed;

                inputRotation.y = ClampAngle(inputRotation.y, minXRotAngle, maxXRotAngle);
            }
        }

        private void RotationUpdate()
        {
            // Camera look direction vector towards the target
            Vector3 direction = new Vector3(0.0f, 0.0f, -distanceBetweenCameraAndTarget);

            // Value equal to the delta change of our input (mouse or touch) position
            Quaternion newQ = Quaternion.Euler(inputRotation.x, inputRotation.y, 0);

            cameraRotation = Quaternion.Slerp(cameraRotation, newQ, slerpValue);  // let cameraRotation value gradually reach newQ which corresponds to our touch
            cameraPos = cameraRotation * direction + target.position;

            mainCamera.transform.rotation = cameraRotation;
            mainCamera.transform.position = cameraPos;
        }
        #endregion

    }

}
