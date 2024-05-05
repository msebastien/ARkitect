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
        private bool _useObjectNameAsIdentifier = true;

        [SerializeField]
        private string _identifier;
        public string Id => _identifier;

        [SerializeField]
        private int _renderingOrder;

        private CanvasGroup _canvasGroup;
        private RectTransform _parentTransform;
        private RectTransform _rectTransform;

        private Vector2 _pivot = new Vector2(0.0F, 1.0F);

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
            if (_useObjectNameAsIdentifier) _identifier = gameObject.name;

            if (_transitions == null)
                _transitions = gameObject.GetOrAddComponent<UIDefaultPageTransitionAnimations>();

        }

        internal void Init(RectTransform parentTransform)
        {
            _rectTransform = (RectTransform)transform;
            _pivot = _rectTransform.pivot;
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _parentTransform = parentTransform;
            _rectTransform.FillParent(_parentTransform, _pivot);

            // Set order of rendering.
            var siblingIndex = 0;
            for (var i = 0; i < _parentTransform.childCount; i++)
            {
                var child = _parentTransform.GetChild(i);
                var childPage = child.GetComponent<UIPage>();
                siblingIndex = i;
                
                if (childPage == null) continue;
                if (_renderingOrder >= childPage._renderingOrder)
                    continue;

                break;
            }

            _rectTransform.SetSiblingIndex(siblingIndex);

            _canvasGroup.alpha = 0.0f;
        }

        internal void BeforeEnter(bool push)
        {
            IsTransitioning = true;
            _transitionAnimationType =
                push ? PageTransitionAnimationType.PushEnter : PageTransitionAnimationType.PopEnter;
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform, _pivot);
            _canvasGroup.alpha = 0.0f; // transparent
        }

        internal AsyncProcessHandle Enter(bool playAnimation = true)
        {
            return CoroutineManager.Instance.Run(EnterRoutine(playAnimation));
        }

        private IEnumerator EnterRoutine(bool playAnimation)
        {
            _canvasGroup.alpha = 1.0f; // visible

            if (playAnimation)
            {
                switch (_transitionAnimationType)
                {
                    case PageTransitionAnimationType.PushEnter:
                        yield return _transitions.AnimatePushEnter(this);
                        break;
                    case PageTransitionAnimationType.PopEnter:
                        yield return _transitions.AnimatePopEnter(this);
                        break;
                    default:
                        break;
                }
            }

            _rectTransform.FillParent(_parentTransform, _pivot);
        }

        internal void AfterEnter()
        {
            IsTransitioning = false;
            _transitionAnimationType = PageTransitionAnimationType.None;
        }

        internal void BeforeExit(bool push)
        {
            IsTransitioning = true;
            _transitionAnimationType = push ? PageTransitionAnimationType.PushExit : PageTransitionAnimationType.PopExit;
            gameObject.SetActive(true);
            _rectTransform.FillParent(_parentTransform, _pivot);
            _canvasGroup.alpha = 1.0f;
        }

        internal AsyncProcessHandle Exit(bool playAnimation = true)
        {
            return CoroutineManager.Instance.Run(ExitRoutine(playAnimation));
        }

        private IEnumerator ExitRoutine(bool playAnimation)
        {
            if (playAnimation)
            {
                switch (_transitionAnimationType)
                {
                    case PageTransitionAnimationType.PushExit:
                        yield return _transitions.AnimatePushExit(this);
                        break;
                    case PageTransitionAnimationType.PopExit:
                        yield return _transitions.AnimatePopExit(this);
                        break;
                    default:
                        break;
                }
            }

            _canvasGroup.alpha = 0.0f; // transparent/hidden
        }

        internal void AfterExit()
        {
            gameObject.SetActive(false);
            IsTransitioning = false;
            _transitionAnimationType = PageTransitionAnimationType.None;
        }
    }

}
