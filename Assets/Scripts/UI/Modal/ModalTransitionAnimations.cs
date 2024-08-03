using System.Collections;
using UnityEngine;

using ARKitect.Coroutine;
using ARKitect.Core;

namespace ARKitect.UI.Modal
{
    public abstract class ModalTransitionAnimations : MonoBehaviour, IModalTransitionAnimations
    {
        [SerializeField]
        [Tooltip("Animation duration in seconds")]
        protected float duration = 0.5F;

        public AsyncProcessHandle AnimateEnter(UIModal modal)
        {
            return ARKitectApp.CoroutineManager.Run(AnimateEnterRoutine(modal));
        }

        protected abstract IEnumerator AnimateEnterRoutine(UIModal modal);

        public AsyncProcessHandle AnimateExit(UIModal modal)
        {
            return ARKitectApp.CoroutineManager.Run(AnimateExitRoutine(modal));
        }

        protected abstract IEnumerator AnimateExitRoutine(UIModal modal);
    }

}