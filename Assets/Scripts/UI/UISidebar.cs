using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace ARKitect.UI
{
    [AddComponentMenu("ARkitect/UI/Sidebar")]
    public class UISidebar : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField]
        private RectTransform sidebar;
        [SerializeField]
        [Tooltip("Used for darkening the background when opening the sidebar. If null, nothing happens.")]
        private Image background;

        [Header("Config")]
        [Tooltip("Delay, in seconds, after which the sidebar panel closes automatically")]
        public float autoCloseDelay = 7.0f;

        [Tooltip("UI Background alpha when the sidebar is open")]
        public float alpha = 0.8F;

        private bool isOpen = false;

        [Header("Events")]
        [SerializeField]
        private UnityEvent<bool> _onToggleSidebarEvent;
        public UnityEvent<bool> OnToggleSidebar => _onToggleSidebarEvent;

        private void Awake()
        {
            if (sidebar == null) sidebar = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            UIDetectBackgroundClick.Instance.OnBackgroundClick.AddListener(OnUIBackgroundClick);
        }

        private void OnDisable()
        {
            UIDetectBackgroundClick.Instance.OnBackgroundClick.RemoveListener(OnUIBackgroundClick);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            if (background != null) background.DOFade(alpha, 0.25F);

            sidebar.DOAnchorPosX(0.0F, 0.4F)
                .From(new Vector2(-sidebar.sizeDelta.x, 0.0F))
                .OnComplete(() =>
                {
                    isOpen = true;
                    _onToggleSidebarEvent.Invoke(isOpen);
                    StartCoroutine(WaitForAutoClose());
                });
        }

        public void Close()
        {
            if (background != null) background.DOFade(0.0F, 0.25F);

            sidebar.DOAnchorPosX(-sidebar.sizeDelta.x, 0.4F)
                .From(Vector2.zero)
                .OnComplete(() =>
                {
                    isOpen = false;
                    _onToggleSidebarEvent.Invoke(isOpen);
                    gameObject.SetActive(false); // also stop coroutines
                });
        }

        private void OnUIBackgroundClick()
        {
            if (isOpen) Close();
        }

        private IEnumerator WaitForAutoClose()
        {
            yield return new WaitForSeconds(autoCloseDelay);
            if (isOpen) Close();
        }
    }

}