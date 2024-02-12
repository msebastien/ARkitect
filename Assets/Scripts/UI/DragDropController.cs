using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace UI
{
    public class DragDropController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public bool resetPositionRelease = true;
        private Vector3 startPosition;

        public void OnBeginDrag(ExtendedPointerEventData eventData)
        {
#if DEBUG
            Debug.Log($"Begin Drag {eventData.position}");
#endif
            if(resetPositionRelease)
                startPosition = eventData.position;
        }

        public void OnDrag(ExtendedPointerEventData eventData)
        {
#if DEBUG
            Debug.Log($"Dragging {eventData.position}");
#endif
            transform.position = eventData.position;
        }

        public void OnEndDrag(ExtendedPointerEventData eventData)
        {
#if DEBUG
            Debug.Log($"End Drag {eventData.position}");
#endif
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, hits);

            var hit = hits.FirstOrDefault(t => t.gameObject.CompareTag("Droppable"));
            if(hit.isValid)
            {
#if DEBUG
                Debug.Log($"Dropped {gameObject} on {hit.gameObject}");
#endif
                return;
            }

            if(resetPositionRelease)
            {
                transform.position = startPosition;
            }
        }
    }

}