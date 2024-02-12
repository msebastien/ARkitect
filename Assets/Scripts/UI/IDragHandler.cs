using UnityEngine.InputSystem.UI;

namespace UI
{
    public interface IDragHandler
    {
        public void OnDrag(ExtendedPointerEventData eventData);
    }

}