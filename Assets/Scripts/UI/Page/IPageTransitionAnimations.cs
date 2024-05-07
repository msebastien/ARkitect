using ARKitect.Coroutine;

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
