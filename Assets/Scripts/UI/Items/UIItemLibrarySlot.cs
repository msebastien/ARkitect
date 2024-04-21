using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

using ARKitect.Items;
using Logger = ARKitect.Core.Logger;


namespace ARKitect.UI.Items
{
    /// <summary>
    /// Manage a item slot in library
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Slots/Item library slot")]
    public class UIItemLibrarySlot : UISlot, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }

}