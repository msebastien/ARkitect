using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using Logger = ARKitect.Core.Logger;

namespace ARKitect.UI.Page
{
    [AddComponentMenu("ARkitect/UI/Page/Default Transitions")]
    public class UIDefaultPageTransitionAnimations : UIAbstractPageTransitionAnimations
    {
        protected override IEnumerator AnimatePushEnterRoutine(UIPage page)
        {
            if (page == null) yield break;
            
            var rectTransform = (RectTransform)page.transform;
            yield return rectTransform
                .DOAnchorPosX(0.0F, 1.0F)
                .From(new Vector2(rectTransform.sizeDelta.x, 0.0F))
                .WaitForCompletion();
            Logger.LogInfo($"{page.Id}: AnimatePushEnter");
        }

        protected override IEnumerator AnimatePushExitRoutine(UIPage page)
        {
            if (page == null) yield break;

            var rectTransform = (RectTransform)page.transform;
            yield return rectTransform
                .DOAnchorPosX(-rectTransform.sizeDelta.x, 1.0F)
                .From(Vector2.zero)
                .WaitForCompletion();
            Logger.LogInfo($"{page.Id}: AnimatePushExit");
        }



        protected override IEnumerator AnimatePopEnterRoutine(UIPage page)
        {
            if (page == null) yield break;

            var rectTransform = (RectTransform)page.transform;
            yield return rectTransform
                .DOAnchorPosX(0.0F, 1.0F)
                .From(new Vector2(-rectTransform.sizeDelta.x, 0.0F))
                .WaitForCompletion();
            Logger.LogInfo($"{page.Id}: AnimatePopEnter");
        }

        protected override IEnumerator AnimatePopExitRoutine(UIPage page)
        {
            if (page == null) yield break;

            var rectTransform = (RectTransform)page.transform;
            yield return rectTransform
                .DOAnchorPosX(rectTransform.sizeDelta.x, 1.0F)
                .From(Vector2.zero)
                .WaitForCompletion();
            Logger.LogInfo($"{page.Id}: AnimatePopExit");
        }
    }

}
