using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

using ARKitect.UI;
using System;

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
        private float mouseRotateSpeed = 0.8f;
        [Range(0.01f, 100)]
        [SerializeField]
        [Tooltip("How sensitive the touch drag to camera rotation")]
        private float touchRotateSpeed = 17.5f;
        [SerializeField]
        [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
        private float slerpValue = 0.25f;
        [SerializeField]
        private float minXRotAngle = -90; // min angle around x axis
        [SerializeField]
        private float maxXRotAngle = 90; // max angle around x axis*

        //private float distanceBetweenCameraAndTarget;

        // Touch rotation
        private Vector2 swipeDirection; // swipe delta vector2
        private Quaternion cameraRot; // store the quaternion after the slerp operation
        private TouchState touch;

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
            //distanceBetweenCameraAndTarget = Vector3.Distance(mainCamera.transform.position, target.position);

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
#if UNITY_EDITOR
                RotateCameraMouse();
#elif UNITY_ANDROID
                RotateCameraTouch();
#endif
            }
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            RotationUpdateMouse();
#elif UNITY_ANDROID
            RotationUpdateTouch();
#endif
        }

        private void SetupInputActions()
        {
            try
            {
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
            ScrollToZoomActions();
            PinchToZoomActions();
        }

        private void ScrollToZoomActions()
        {
            mouseScrollAction.Enable();
            mouseScrollAction.performed += ctx => ZoomCamera(ctx.ReadValue<Vector2>().y * mouseZoomSpeed);
        }

        private void PinchToZoomActions()
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
            touch1PosAction.performed += _ =>
            {
                if (!enableCameraControls && !moveCamera) return;
                if (touchCount < 2) return;

                var magnitude = (touch0PosAction.ReadValue<Vector2>() - touch1PosAction.ReadValue<Vector2>()).magnitude;

                if (prevMagnitude == 0) prevMagnitude = magnitude;

                var difference = magnitude - prevMagnitude;
                prevMagnitude = magnitude;

                ZoomCamera(difference * touchZoomSpeed);
            };
        }

        private void RotateCameraMouse()
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            mouseRot.x = -mouseDelta.y * mouseRotateSpeed; // around X
            mouseRot.y = mouseDelta.x * mouseRotateSpeed;

            Mathf.Clamp(mouseRot.x, minXRotAngle, maxXRotAngle);
        }

        private void RotateCameraTouch()
        {
            touch = Touchscreen.current.primaryTouch.ReadValue();

            if (Touchscreen.current.primaryTouch.phase.value == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Logger.LogInfo("Camera Controller: Touch Began");
            }
            else if (Touchscreen.current.primaryTouch.phase.value == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                swipeDirection += touch.delta * Time.deltaTime * touchRotateSpeed;
            }
            else if (Touchscreen.current.primaryTouch.phase.value == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                Logger.LogInfo("Camera Controller: Touch Ended");
            }

            Mathf.Clamp(swipeDirection.y, minXRotAngle, maxXRotAngle);
        }

        private void RotationUpdateMouse()
        {
            // Value equal to the delta change of our mouse position
            Quaternion newQ = Quaternion.Euler(mouseRot.x, mouseRot.y, 0); // We are setting the rotation around X, Y, Z axis respectively

            cameraRot = Quaternion.Slerp(cameraRot, newQ, slerpValue);  //let cameraRot value gradually reach newQ which corresponds to our touch
            mainCamera.transform.position = target.position + cameraRot * (mainCamera.transform.position - target.position);
            mainCamera.transform.LookAt(target.position);
        }

        private void RotationUpdateTouch()
        {
            // Value equal to the delta change of our touch position
            Quaternion newQ = Quaternion.Euler(swipeDirection.y, -swipeDirection.x, 0);

            cameraRot = Quaternion.Slerp(cameraRot, newQ, slerpValue);  //let cameraRot value gradually reach newQ which corresponds to our touch
            mainCamera.transform.position = target.position + cameraRot * (mainCamera.transform.position - target.position);
            mainCamera.transform.LookAt(target.position);
        }

        private void ZoomCamera(float increment) => mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + increment, minFOV, maxFOV);
    }

}
