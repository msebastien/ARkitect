using ARKitect.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARkitect.UI
{
    [AddComponentMenu("ARkitect/UI/Toggle Camera Controls")]
    public class UIToggleCameraControls : MonoBehaviour
    {
        [SerializeField]
        private CameraController _cameraController;

        private void Awake()
        {
            if (_cameraController == null)
                _cameraController = GetComponent<CameraController>();
        }

        public void DisableCameraControlWhenEnablingSidebar(bool sidebarToggle)
        {
            _cameraController.EnableCameraControls = !sidebarToggle;
        }
    }

}
