using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

using ARKitect.UI;
using Unity.VisualScripting;
using UnityEngine.UIElements;

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
        public bool EnableCameraControls
        {
            get { return enableCameraControls; }
            set { enableCameraControls = value; }
        }

        [Header("Rotation")]
        [Range(0.1f, 5f)]
        [SerializeField]
        [Tooltip("How sensitive the mouse drag to camera rotation")]
        private float mouseRotateSpeed = 0.7f;
        [Range(0.01f, 100)]
        [SerializeField]
        [Tooltip("How sensitive the touch drag to camera rotation")]
        private float touchRotateSpeed = 0.7f;
        [SerializeField]
        [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
        private float slerpValue = 0.25f;
        [SerializeField]
        private float minXRotAngle = -90; // min angle around x axis
        [SerializeField]
        private float maxXRotAngle = 90; // max angle around x axis

        private float distanceBetweenCameraAndTarget;
        private Quaternion cameraRotation; // store the quaternion after the slerp operation
        private Vector3 cameraPos;

        private InputAction rotateAction;
        private Vector2 inputDelta;
        enum RotateMethod { Mouse, Touch };
        private RotateMethod rotateMethod = RotateMethod.Mouse;

        // Touch rotation
        private Vector2 swipeDirection; // swipe delta vector2   

        // Mouse rotation related
        private Vector2 mouseRot;

        [Header("Zoom")]
        [SerializeField]
        private float mouseZoomSpeed = 0.05f;
        [SerializeField]
        private float touchZoomSpeed = 0.05f;
        [SerializeField]
        private float minFOV = 20.0f;
        [SerializeField]
        private float maxFOV = 70.0f;

        // Mouse Zoom
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
            mainCamera.transform.LookAt(target);
            distanceBetweenCameraAndTarget = Vector3.Distance(mainCamera.transform.position, target.position);

            SetupInputActions();
            ProcessInputActions();

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

        private void Update()
        {
            if (enableCameraControls && moveCamera)
            {
                RotateCamera();
            }
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

        #region Zoom

        private void ScrollToZoomActions()
        {
            mouseScrollAction.Enable();
            mouseScrollAction.performed += ctx => ZoomCamera(ctx.ReadValue<Vector2>().y * mouseZoomSpeed);
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

            ZoomCamera(difference * touchZoomSpeed);
        }

        private void ZoomCamera(float increment) => mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + increment, minFOV, maxFOV);

        #endregion

        #region Rotation        

        private void RotateCameraAction()
        {
            rotateAction.Enable();

            rotateAction.performed += ctx =>
            {
                if (ctx.control.device is Mouse)
                    rotateMethod = RotateMethod.Mouse;
                else if (ctx.control.device is Touchscreen)
                    rotateMethod = RotateMethod.Touch;
            };
        }

        private void RotateCamera()
        {
            if (rotateMethod == RotateMethod.Mouse)
                RotateCameraMouse();
            else
                RotateCameraTouch();
        }

        private void RotateCameraMouse()
        {
            inputDelta = rotateAction.ReadValue<Vector2>();
            mouseRot.x += -inputDelta.y * mouseRotateSpeed; // around X
            mouseRot.y += inputDelta.x * mouseRotateSpeed;

            mouseRot.x = Mathf.Clamp(mouseRot.x, minXRotAngle, maxXRotAngle);
        }

        private void RotateCameraTouch()
        {
            if (touchCount > 1) return;
            
            inputDelta = rotateAction.ReadValue<Vector2>();

            if (Touchscreen.current.primaryTouch.phase.value == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Logger.LogInfo("Camera Controller: Touch Began");
            }
            else if (Touchscreen.current.primaryTouch.phase.value == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                swipeDirection += inputDelta * touchRotateSpeed;
            }
            else if (Touchscreen.current.primaryTouch.phase.value == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                Logger.LogInfo("Camera Controller: Touch Ended");
            }

            swipeDirection.y = Mathf.Clamp(swipeDirection.y, minXRotAngle, maxXRotAngle);
        }

        private void RotationUpdate()
        {
            Quaternion newQ;
            if (rotateMethod == RotateMethod.Mouse)
            {
                // Value equal to the delta change of our mouse position
                newQ = Quaternion.Euler(mouseRot.x, mouseRot.y, 0); // We are setting the rotation around X, Y, Z axis respectively
            }
            else
            {
                // Value equal to the delta change of our touch position
                newQ = Quaternion.Euler(swipeDirection.y, -swipeDirection.x, 0);
            }

            cameraRotation = Quaternion.Slerp(cameraRotation, newQ, slerpValue);  //let cameraRot value gradually reach newQ which corresponds to our touch
            Vector3 cameraPos = cameraRotation * new Vector3(0.0f, 0.0f, -distanceBetweenCameraAndTarget) + target.position;

            mainCamera.transform.rotation = cameraRotation;
            mainCamera.transform.position = cameraPos;
        }
        #endregion

    }

}
