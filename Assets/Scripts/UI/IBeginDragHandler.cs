using UnityEngine.InputSystem.UI;

namespace UI
{
    public interface IBeginDragHandler
    {
        public void OnBeginDrag(ExtendedPointerEventData eventData);
    }

}