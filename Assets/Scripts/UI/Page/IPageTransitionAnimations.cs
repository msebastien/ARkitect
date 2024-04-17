using ARKitect.Coroutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARKitect.UI.Page
{
    public interface IPageTransitionAnimations
    {
        public AsyncProcessHandle AnimatePushEnter(UIPage page);
        public AsyncProcessHandle AnimatePushExit(UIPage page);
        public AsyncProcessHandle AnimatePopEnter(UIPage page);
        public AsyncProcessHandle AnimatePopExit(UIPage page);
    }

}
