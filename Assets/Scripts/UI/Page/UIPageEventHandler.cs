using UnityEngine;
using UnityEngine.UI;

namespace ARKitect.UI.Page
{
    /// <summary>
    /// This script allows to easily push/pop pages by registering simple methods to UI elements' events, such as buttons with the OnClick() event. 
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Page/Page Event Handler")]
    public class UIPageEventHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Enable the automatic subscribe to the OnClick() event of a button")]
        private bool _autoSubscribeToButtonClickEvent = false;

        [SerializeField]
        [Tooltip("Reference to the page container managing all the pages")]
        private UIPageContainer _container;

        [SerializeField]
        [Tooltip("The new page to push to display")]
        private UIPage _page;

        private Button _button;

        private void Awake()
        {
            if (_container == null) _container = UIPageContainer.Instance;

            if (_autoSubscribeToButtonClickEvent) _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (_autoSubscribeToButtonClickEvent)
            {
                if (_button == null) return;

                if (_page != null)
                    _button.onClick.AddListener(Push);
                else
                    _button.onClick.AddListener(Pop);
            }
        }

        private void OnDisable()
        {
            if (_autoSubscribeToButtonClickEvent)
            {
                if (_button == null) return;

                if (_page != null)
                    _button.onClick.RemoveListener(Push);
                else
                    _button.onClick.RemoveListener(Pop);
            }
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
