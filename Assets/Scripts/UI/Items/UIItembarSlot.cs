using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;


using ARKitect.Items;
using ARKitect.Core;
using Logger = ARKitect.Core.Logger;



namespace ARKitect.UI.Items
{
    /// <summary>
    /// Manage a item slot in shortcut bar
    /// </summary>
    public class UIItemBarSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private Image icon;

        private ItemsController itemBar;

        public byte index;

        public void Bind(ItemsController itemBar, byte index)
        {
            this.index = index;
            this.itemBar = itemBar;
            Identifier itemDefinitionId = itemBar.ItemDefinitionsInSlots.Count > index ? itemBar.ItemDefinitionsInSlots[index] : new Identifier();
            ShowItemDefinition(itemDefinitionId);
        }

        public void RefreshItemVisuals()
        {
            Identifier itemDefinitionId = itemBar.ItemDefinitionsInSlots.Count > index ? itemBar.ItemDefinitionsInSlots[index] : new Identifier();
            ShowItemDefinition(itemDefinitionId);
        }

        public void ShowItemDefinition(Identifier itemDefinitionId)
        {
            var itemDefinition = PrefabsManager.Instance.Items[itemDefinitionId];

            if (itemDefinition == null) { Logger.LogError($"Item {itemDefinitionId} not found in catalog."); }

            if (icon == null) { Logger.LogError("Icon is null"); return; }

            icon.sprite = itemDefinition?.Icon;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            DragIcon.Instance.SetSprite(icon.sprite);
            DragIcon.Instance.gameObject.SetActive(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragIcon.Instance.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DropOnItemSlot(eventData) ||
                DropOnGround())
            {
                DragIcon.Instance.SetSprite(null);
                DragIcon.Instance.gameObject.SetActive(false);
            }
        }

        private bool DropOnGround()
        {
            var ray = UnityEngine.Camera.main.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
            if(Physics.Raycast(ray, out var hit)) 
            {
                // TODO: Spawn Item's prefab
                itemBar.Spawn(hit.point);
                return true;
            }
            return false;
        }

        private bool DropOnItemSlot(PointerEventData eventData)
        {
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, hits);

            foreach(var hit in hits)
            {
                var droppedSlot = hit.gameObject.GetComponent<UIItemBarSlot>();
                if(droppedSlot)
                {
                    Logger.LogInfo($"Drag End {droppedSlot?.name}");
                    Swap(this, droppedSlot);

                    return true;
                }
            }

            return false;
        }

        public void Swap(UIItemBarSlot slot1, UIItemBarSlot slot2)
        {
            itemBar.Swap(slot1.index, slot2.index);
        }
        

        
    }

}