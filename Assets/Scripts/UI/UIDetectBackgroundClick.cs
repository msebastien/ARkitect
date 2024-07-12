using ARKitect.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Logger = ARKitect.Core.Logger;

namespace ARKitect.UI
{
    [AddComponentMenu("ARkitect/UI/Detect Background Click")]
    public class UIDetectBackgroundClick : Singleton<UIDetectBackgroundClick>, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private UnityEvent backgroundClickEvent;
        [SerializeField]
        private UnityEvent backgroundPressEvent;
        [SerializeField]
        private UnityEvent backgroundReleaseEvent;

        public UnityEvent OnBackgroundClick => backgroundClickEvent;
        public UnityEvent OnBackgroundPress => backgroundPressEvent;
        public UnityEvent OnBackgroundRelease => backgroundReleaseEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            Logger.LogInfo("UI Background clicked!");
            backgroundClickEvent.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Logger.LogInfo("UI Background pressed!");
            backgroundPressEvent.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            backgroundReleaseEvent.Invoke();
        }
    }

}