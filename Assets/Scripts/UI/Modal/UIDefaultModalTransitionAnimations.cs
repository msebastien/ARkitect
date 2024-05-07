using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ARKitect.UI.Modal
{
    [AddComponentMenu("ARkitect/UI/Modal/Default Transitions")]
    public class UIDefaultModalTransitionAnimations : ModalTransitionAnimations
    {
        protected override IEnumerator AnimateEnterRoutine(UIModal modal)
        {
            if (modal == null) yield break;

            var rectTransform = (RectTransform)modal.transform;
            yield return rectTransform
                .DOPunchScale(new Vector2(-0.1F, -0.1F), duration, 2, 0.25F)
                .WaitForCompletion();
        }

        protected override IEnumerator AnimateExitRoutine(UIModal modal)
        {
            if (modal == null) yield break;

            var canvasGroup = modal.gameObject.GetComponent<CanvasGroup>();
            yield return canvasGroup
                .DOFade(0.0F, duration)
                .From(1.0F)
                .OnComplete(() => modal.Alpha = 0.0F) // transparent/hidden
                .WaitForCompletion();
        }
    }

}
