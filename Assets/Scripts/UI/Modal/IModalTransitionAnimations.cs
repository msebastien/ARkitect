using ARKitect.Coroutine;

namespace ARKitect.UI.Modal
{
    public interface IModalTransitionAnimations
    {
        public AsyncProcessHandle AnimateEnter(UIModal modal);
        public AsyncProcessHandle AnimateExit(UIModal modal);
    }

    public interface IModalBackdropTransitionAnimations
    {
        public AsyncProcessHandle AnimateBackdropEnter(UIModalBackdrop backdrop);
        public AsyncProcessHandle AnimateBackdropExit(UIModalBackdrop backdrop);
    }
}
