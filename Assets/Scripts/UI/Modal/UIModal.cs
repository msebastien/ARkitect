using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Sirenix.OdinInspector;

using ARKitect.Coroutine;
using ARKitect.Core;


namespace ARKitect.UI.Modal
{
    [AddComponentMenu("ARkitect/UI/Modal/Modal")]
    public class UIModal : MonoBehaviour
    {
        [SerializeField]
        private bool _useObjectNameAsIdentifier = true;

        [SerializeField]
        private string _identifier;
        public string Id => _identifier;

        [SerializeField]
        private int _renderingOrder;

        private CanvasGroup _canvasGroup;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;

        public float Alpha
        {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }

        public bool IsTransitioning { get; private set; }

        /// <summary>
        ///     Return the transition animation type currently playing.
        ///     If not in transition, return ModalTransitionAnimationType.None
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private ModalTransitionAnimationType _transitionAnimationType;
        public ModalTransitionAnimationType TransitionAnimationType => _transitionAnimationType;

        [SerializeField]
        private ModalTransitionAnimations _transitions;

        private void Awake()
        {
            if (_useObjectNameAsIdentifier) _identifier = gameObject.name;

            if (_transitions == null)
                _transitions = gameObject.GetOrAddComponent<UIDefaultModalTransitionAnimations>();
        }

        internal void Init(RectTransform parentTransform)
        {
            _rectTransform = (RectTransform)transform;
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _parentTransform = parentTransform;
            _rectTransform.FillParent(_parentTransform);

            // Set order of rendering.
            var siblingIndex = 0;
            for (var i = 0; i < _parentTransform.childCount; i++)
            {
                var child = _parentTransform.GetChild(i);
                var childModal = child.GetComponent<UIModal>();
                var backdrop = child.GetComponent<UIModalBackdrop>();

                siblingIndex = i;

                // Set backdrop sibling index to avoid it being dislay over the Modal
                if (backdrop != null) backdrop.transform.SetSiblingIndex(siblingIndex);

                if (childModal == null) continue;
                if (_renderingOrder >= childModal._renderingOrder)
                    continue;

                break;
            }

            // Set the current modal with the highest sibling index, which
            // means this is the top level visual element
            _rectTransform.SetSiblingIndex(siblingIndex);

            _canvasGroup.alpha = 0.0f; // transparent/hidden
        }

        internal void BeforeEnter(bool push, UIModal partnerModal)
        {
            IsTransitioning = true;
            if (push) // only a pushed modal can have a "Enter" animation
            {
                _transitionAnimationType = ModalTransitionAnimationType.Enter;
                gameObject.SetActive(true);
                _rectTransform.FillParent(_parentTransform);
                _canvasGroup.alpha = 0.0f; // transparent
            }
        }

        internal AsyncProcessHandle Enter(UIModal partnerModal, bool playAnimation = true)
        {
            return ARKitectApp.Instance.CoroutineManager.Run(EnterRoutine(partnerModal, playAnimation));
        }

        private IEnumerator EnterRoutine(UIModal partnerModal, bool playAnimation)
        {
            if (_transitionAnimationType == ModalTransitionAnimationType.Enter)
            {
                _canvasGroup.alpha = 1.0f; // visible

                if (playAnimation)
                {
                    yield return _transitions.AnimateEnter(partnerModal);
                }

                _rectTransform.FillParent(_parentTransform);
            }
        }

        internal void AfterEnter(UIModal partnerModal)
        {
            IsTransitioning = false;
            _transitionAnimationType = ModalTransitionAnimationType.None;
        }

        internal void BeforeExit(bool push, UIModal partnerModal)
        {
            IsTransitioning = true;
            if (!push) // only a popped modal can have a "Exit" animation
            {
                _transitionAnimationType = ModalTransitionAnimationType.Exit;
                gameObject.SetActive(true);
                _rectTransform.FillParent(_parentTransform);
                _canvasGroup.alpha = 1.0f; // visible
            }
        }

        internal AsyncProcessHandle Exit(UIModal partnerModal, bool playAnimation = true)
        {
            return ARKitectApp.Instance.CoroutineManager.Run(ExitRoutine(partnerModal, playAnimation));
        }

        private IEnumerator ExitRoutine(UIModal partnerModal, bool playAnimation)
        {
            if (_transitionAnimationType == ModalTransitionAnimationType.Exit)
            {
                if (playAnimation)
                {
                    yield return _transitions.AnimateExit(partnerModal);
                }

                //_canvasGroup.alpha = 0.0f; // transparent/hidden
            }
        }

        internal void AfterExit(UIModal partnerModal)
        {
            IsTransitioning = false;
            _transitionAnimationType = ModalTransitionAnimationType.None;
        }
    }

}