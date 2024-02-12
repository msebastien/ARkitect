using UnityEngine.InputSystem.UI;

namespace UI
{
    public interface IEndDragHandler
    {
        public void OnEndDrag(ExtendedPointerEventData eventData);
    }

}