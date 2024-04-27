using ARKitect.Coroutine;
using System.Collections;
using UnityEngine;

namespace ARKitect.UI.Page
{
    public abstract class PageTransitionAnimations : MonoBehaviour, IPageTransitionAnimations
    {
        [SerializeField]
        [Tooltip("Animation duration in seconds")]
        protected float duration = 1.0F;

        public AsyncProcessHandle AnimatePushEnter(UIPage page)
        {
            return CoroutineManager.Instance.Run(AnimatePushEnterRoutine(page));
        }

        protected abstract IEnumerator AnimatePushEnterRoutine(UIPage page);

        public AsyncProcessHandle AnimatePushExit(UIPage page)
        {
            return CoroutineManager.Instance.Run(AnimatePushExitRoutine(page));
        }

        protected abstract IEnumerator AnimatePushExitRoutine(UIPage page);



        public AsyncProcessHandle AnimatePopEnter(UIPage page)
        {
            return CoroutineManager.Instance.Run(AnimatePopEnterRoutine(page));
        }

        protected abstract IEnumerator AnimatePopEnterRoutine(UIPage page);

        public AsyncProcessHandle AnimatePopExit(UIPage page)
        {
            return CoroutineManager.Instance.Run(AnimatePopExitRoutine(page));
        }

        protected abstract IEnumerator AnimatePopExitRoutine(UIPage page);

        
    }

}
