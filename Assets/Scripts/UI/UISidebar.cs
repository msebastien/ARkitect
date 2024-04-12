using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using Logger = ARKitect.Core.Logger;

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

        [Tooltip("UI Background alpha when the sidebar is opened")]
        public float alpha = 0.8F;

        private bool isOpen = false;

        private void Awake()
        {
            if (sidebar == null) sidebar = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            UIDetectBackgroundClick.OnBackgroundClick.AddListener(OnUIBackgroundClick);
        }

        private void OnDisable() 
        {
            UIDetectBackgroundClick.OnBackgroundClick.RemoveListener(OnUIBackgroundClick);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            if (background != null) background.DOFade(alpha, 0.25F);

            sidebar.DOAnchorPosX(0.0F, 0.75F)
                .OnComplete(() => { 
                    isOpen = true;
                    StartCoroutine(WaitForAutoClose());
                });
        }

        public void Close()
        {
            if (background != null) background.DOFade(0.0F, 0.25F);

            sidebar.DOAnchorPosX(-sidebar.sizeDelta.x, 0.75F)
                .From(Vector2.zero)
                .OnComplete(() => {
                    isOpen = false;
                    gameObject.SetActive(false);
                });
        }

        private void OnUIBackgroundClick()
        {
            if(isOpen) Close();
        }

        private IEnumerator WaitForAutoClose()
        {
            yield return new WaitForSeconds(autoCloseDelay);
            if (isOpen) Close();
        }
    }

}