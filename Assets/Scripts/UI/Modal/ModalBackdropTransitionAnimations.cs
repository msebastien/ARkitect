using ARKitect.Core;
using ARKitect.Coroutine;
using System.Collections;
using UnityEngine;

namespace ARKitect.UI.Modal
{
    public abstract class ModalBackdropTransitionAnimations : MonoBehaviour, IModalBackdropTransitionAnimations
    {
        [SerializeField]
        [Tooltip("Animation duration in seconds")]
        protected float duration = 0.5F;
        
        [SerializeField]
        [Tooltip("Modal Backdrop alpha when the modal is open")]
        protected float alpha = 0.8F;

        public AsyncProcessHandle AnimateBackdropEnter(UIModalBackdrop backdrop)
        {
            return ARKitectApp.CoroutineManager.Run(AnimateBackdropEnterRoutine(backdrop));
        }

        protected abstract IEnumerator AnimateBackdropEnterRoutine(UIModalBackdrop backdrop);

        public AsyncProcessHandle AnimateBackdropExit(UIModalBackdrop backdrop)
        {
            return ARKitectApp.CoroutineManager.Run(AnimateBackdropExitRoutine(backdrop));
        }

        protected abstract IEnumerator AnimateBackdropExitRoutine(UIModalBackdrop backdrop);
    }

}
