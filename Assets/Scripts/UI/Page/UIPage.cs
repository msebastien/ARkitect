using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Sirenix.OdinInspector;

using ARKitect.Coroutine;
using Logger = ARKitect.Core.Logger;


namespace ARKitect.UI.Page
{
    [AddComponentMenu("ARkitect/UI/Page/Page")]
    public class UIPage : MonoBehaviour
    {
        [SerializeField]
        private string _identifier;
        public string Id 
        { 
            get => _identifier;
        }

        [SerializeField] 
        private int _renderingOrder;

        private CanvasGroup _canvasGroup;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;

        public bool IsTransitioning { get; private set; }

        /// <summary>
        ///     Return the transition animation type currently playing.
        ///     If not in transition, return PageTransitionAnimationType.None
        /// </summary>
        [SerializeField]
        [ReadOnly]
        private PageTransitionAnimationType _transitionAnimationType;
        public PageTransitionAnimationType TransitionAnimationType => _transitionAnimationType;

        [SerializeField]
        private PageTransitionAnimations _transitions;

        private void Awake()
        {
            if(_transitions == null)
                _transitions = gameObject.GetOrAddComponent<PageTransitionAnimations>();
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
                var childPage = child.GetComponent<UIPage>();
                siblingIndex = i;
                if (_renderingOrder >= childPage._renderingOrder)
                    continue;

                break;
            }

            _rectTransform.SetSiblingIndex(siblingIndex);

            _canvasGroup.alpha = 0.0f;
        }

        internal void BeforeEnter(bool push, UIPage partnerPage)
        {
            IsTransitioning = true;
            _transitionAnimationType =
                push ? PageTransitionAnimationType.PushEnter : PageTransitionAnimationType.PopEnter;
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 0.0f; // transparent
        }

        internal AsyncProcessHandle Enter(UIPage partnerPage, bool playAnimation = true)
        {
            return CoroutineManager.Instance.Run(EnterRoutine(partnerPage, playAnimation));
        }

        private IEnumerator EnterRoutine(UIPage partnerPage, bool playAnimation)
        {
            _canvasGroup.alpha = 1.0f; // visible

            if (playAnimation)
            {
                switch(_transitionAnimationType)
                {
                    case PageTransitionAnimationType.PushEnter:
                        yield return _transitions.AnimatePushEnter(partnerPage);
                        break;
                    case PageTransitionAnimationType.PopEnter:
                        yield return _transitions.AnimatePopEnter(partnerPage);
                        break;
                    default:
                        break;
                }
            }

            _rectTransform.FillParent(_parentTransform);
        }

        internal void AfterEnter(UIPage partnerPage)
        {
            IsTransitioning = false;
            _transitionAnimationType = PageTransitionAnimationType.None;
        }

        internal void BeforeExit(bool push, UIPage partnerPage)
        {
            IsTransitioning = true;
            _transitionAnimationType = push ? PageTransitionAnimationType.PushExit : PageTransitionAnimationType.PopExit;
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform);
            _canvasGroup.alpha = 1.0f;
        }

        internal AsyncProcessHandle Exit(UIPage partnerPage, bool playAnimation = true)
        {
            return CoroutineManager.Instance.Run(ExitRoutine(partnerPage, playAnimation));
        }

        private IEnumerator ExitRoutine(UIPage partnerPage, bool playAnimation)
        {
            if(playAnimation)
            {
                switch (_transitionAnimationType)
                {
                    case PageTransitionAnimationType.PushExit:
                        yield return _transitions.AnimatePushExit(partnerPage);
                        break;
                    case PageTransitionAnimationType.PopExit:
                        yield return _transitions.AnimatePopExit(partnerPage);
                        break;
                    default:
                        break;
                }
            }

            _canvasGroup.alpha = 0.0f; // transparent/hidden
        }

        internal void AfterExit(UIPage partnerPage)
        {
            gameObject.SetActive(false);
            IsTransitioning = false;
            _transitionAnimationType = PageTransitionAnimationType.None;
        }
    }

}
