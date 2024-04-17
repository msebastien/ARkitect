using UnityEngine;

namespace ARKitect.UI.Page
{
    /// <summary>
    /// This script allows to easily push/pop pages by registering simple methods to UI elements' events, such as buttons with the OnClick() event. 
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Page/Page Event Handler")]
    public class UIPageEventHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the page container managing all the pages")]
        private UIPageContainer _container;

        [SerializeField]
        [Tooltip("The new page to push to display")]
        private UIPage _page;

        private void Awake()
        {
            if (_container == null) _container = UIPageContainer.Instance;
        }

        /// <summary>
        /// Push the new page to display it
        /// </summary>
        public void Push()
        {
            if (_page != null && _container != null)
            {
                _container.Push(_page.Id);
            }
        }

        /// <summary>
        /// Pop the current active page to go back to the previous one
        /// </summary>
        public void Pop()
        {
            if (_container != null)
            {
                _container.Pop();
            }
        }
    }

}
