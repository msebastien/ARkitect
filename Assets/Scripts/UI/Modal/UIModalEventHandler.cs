using UnityEngine;

namespace ARKitect.UI.Modal
{
    /// <summary>
    /// This script allows to easily push/pop modals by registering simple methods to UI elements' events, such as buttons with the OnClick() event. 
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Modal/Modal Event Handler")]
    public class UIModalEventHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to the modal container managing all the modals")]
        private UIModalContainer _container;

        [SerializeField]
        [Tooltip("The new modal to push to display")]
        private UIModal _modal;
        public UIModal Modal
        {
            get => _modal;
            set => _modal = value;
        }

        private void Awake()
        {
            if (_container == null) _container = UIModalContainer.Instance;
        }

        /// <summary>
        /// Push the new page to display it
        /// </summary>
        public void Push()
        {
            if (_modal != null && _container != null)
            {
                _container.Push(_modal.Id);
            }
        }

        /// <summary>
        /// Pop the current active modal to go back to the previous one
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
