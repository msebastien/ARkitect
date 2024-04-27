using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ARKitect.UI.Modal
{
    [AddComponentMenu("ARkitect/UI/Modal/Backdrop Default Transitions")]
    public class UIDefaultModalBackdropTransitionAnimations : ModalBackdropTransitionAnimations
    {
        protected override IEnumerator AnimateBackdropEnterRoutine(UIModalBackdrop backdrop)
        {
            if (backdrop == null) yield break;

            yield return backdrop.Image
                .DOFade(alpha, duration)
                .From(Color.clear)
                .WaitForCompletion();
        }

        protected override IEnumerator AnimateBackdropExitRoutine(UIModalBackdrop backdrop)
        {
            if (backdrop == null) yield break;

            yield return backdrop.Image
                .DOFade(Color.clear.a, duration)
                .From(alpha)
                .WaitForCompletion();
        }
    }

}
