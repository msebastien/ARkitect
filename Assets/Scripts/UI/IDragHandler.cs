using UnityEngine.InputSystem.UI;

namespace ARKitect.UI
{
    public interface IDragHandler
    {
        public void OnDrag(ExtendedPointerEventData eventData);
    }

}