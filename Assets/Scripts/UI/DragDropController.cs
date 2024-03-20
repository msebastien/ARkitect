using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

using Logger = ARKitect.Core.Logger;


namespace ARKitect.UI
{
    public class DragDropController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public bool resetPositionRelease = true;
        private Vector3 startPosition;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Logger.LogInfo($"Begin Drag {eventData.position}");
            if(resetPositionRelease)
                startPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Logger.LogInfo($"Dragging {eventData.position}");
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Logger.LogInfo($"End Drag {eventData.position}");

            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, hits);

            var hit = hits.FirstOrDefault(t => t.gameObject.CompareTag("Droppable"));
            if(hit.isValid)
            {
                Logger.LogInfo($"Dropped {gameObject} on {hit.gameObject}");
                return;
            }

            if(resetPositionRelease)
            {
                transform.position = startPosition;
            }
        }
    }

}