using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ARKitect.Coroutine;

namespace ARKitect.UI.Modal
{
    [AddComponentMenu("ARkitect/UI/Modal/Backdrop")]
    public class UIModalBackdrop : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private ModalBackdropTransitionAnimations _transitions;

        [SerializeField]
        private bool _closeModalWhenClicked;

        private CanvasGroup _canvasGroup;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;

        [SerializeField]
        private Image _image;
        public Image Image => _image;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            _image = gameObject.GetOrAddComponent<Image>();
            _image.color = Color.clear;

            if (_transitions == null)
                _transitions = gameObject.GetOrAddComponent<UIDefaultModalBackdropTransitionAnimations>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_closeModalWhenClicked)
            {
                var modalContainer = UIModalContainer.Instance;
                if (modalContainer.IsInTransition)
                    return;

                modalContainer.Pop();
            }
        }

        public void Setup(RectTransform parentTransform)
        {
            _parentTransform = parentTransform;
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.interactable = _closeModalWhenClicked;
            gameObject.SetActive(false);
        }

        internal AsyncProcessHandle Enter(bool playAnimation)
        {
            return CoroutineManager.Instance.Run(EnterRoutine(playAnimation));
        }

        private IEnumerator EnterRoutine(bool playAnimation)
        {
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 1.0f; // visible

            if (playAnimation)
            {
                yield return _transitions.AnimateBackdropEnter(this);
            }

            _rectTransform.FillParent(_parentTransform);
        }

        internal AsyncProcessHandle Exit(bool playAnimation)
        {
            return CoroutineManager.Instance.Run(ExitRoutine(playAnimation));
        }

        private IEnumerator ExitRoutine(bool playAnimation)
        {
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 1.0f; // visible

            if (playAnimation)
            {
                yield return _transitions.AnimateBackdropExit(this);
            }

            _canvasGroup.alpha = 0.0f; // hidden
            gameObject.SetActive(false);
        }
    }

}
