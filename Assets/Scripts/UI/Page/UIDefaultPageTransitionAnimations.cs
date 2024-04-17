using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using Logger = ARKitect.Core.Logger;

namespace ARKitect.UI.Page
{
    [AddComponentMenu("ARkitect/UI/Page/Default Transitions")]
    public class UIDefaultPageTransitionAnimations : PageTransitionAnimations
    {
        protected override IEnumerator AnimatePushEnterRoutine(UIPage page)
        {
            if (page == null) yield break;

            var rectTransform = (RectTransform)page.transform;
            yield return rectTransform
                .DOAnchorPosX(0.0F, 1.0F)
                .From(new Vector2(Screen.width, 0.0F))
                .WaitForCompletion();
        }

        protected override IEnumerator AnimatePushExitRoutine(UIPage page)
        {
            if (page == null) yield break;

            var rectTransform = (RectTransform)page.transform;
            yield return rectTransform
                .DOAnchorPosX(-Screen.width, 1.0F)
                .From(Vector2.zero)
                .WaitForCompletion();
        }



        protected override IEnumerator AnimatePopEnterRoutine(UIPage page)
        {
            if (page == null) yield break;

            var rectTransform = (RectTransform)page.transform;
            yield return rectTransform
                .DOAnchorPosX(0.0F, 1.0F)
                .From(new Vector2(-Screen.width, 0.0F))
                .WaitForCompletion();
        }

        protected override IEnumerator AnimatePopExitRoutine(UIPage page)
        {
            if (page == null) yield break;

            var rectTransform = (RectTransform)page.transform;
            yield return rectTransform
                .DOAnchorPosX(Screen.width, 1.0F)
                .From(Vector2.zero)
                .WaitForCompletion();
        }
    }

}
