using ARKitect.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Logger = ARKitect.Core.Logger;

namespace ARKitect.UI
{
    [AddComponentMenu("ARkitect/UI/Detect Background Click")]
    public class UIDetectBackgroundClick : Singleton<UIDetectBackgroundClick>, IPointerClickHandler
    {
        [SerializeField]
        private UnityEvent backgroundClickEvent;
        public static UnityEvent OnBackgroundClick => Instance.backgroundClickEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            Logger.LogInfo("UI Background clicked!");
            backgroundClickEvent.Invoke();
        }
    }

}