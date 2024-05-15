using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

namespace ARKitect.UI.Items
{
    [AddComponentMenu("ARkitect/UI/Item Search")]
    public class UIItemSearch : MonoBehaviour
    {
        [SerializeField]
        private UIItemLibrary _itemLibrary;

        [SerializeField]
        private TMP_InputField _searchField;

        private bool _keepOldTextInField = false;

        [SerializeField]
        [ReadOnly]
        private string _searchText;
        private string _oldEditText;

        private void OnEnable()
        {
            _searchField.onValueChanged.AddListener(OnSearchFieldValueChanged);
            _searchField.onValueChanged.AddListener(Editing);
            _searchField.onEndEdit.AddListener(EndEdit);
            _searchField.onTouchScreenKeyboardStatusChanged.AddListener(ReportChangeStatus);
        }

        private void OnDisable()
        {
            _searchField.onValueChanged.RemoveListener(OnSearchFieldValueChanged);
            _searchField.onValueChanged.RemoveListener(Editing);
            _searchField.onEndEdit.RemoveListener(EndEdit);
            _searchField.onTouchScreenKeyboardStatusChanged.RemoveListener(ReportChangeStatus);
        }

        private void ReportChangeStatus(TouchScreenKeyboard.Status newStatus)
        {
            if (newStatus == TouchScreenKeyboard.Status.Canceled)
                _keepOldTextInField = true;
        }

        private void OnSearchFieldValueChanged(string query)
        {
            _itemLibrary.FilterBySearch(query);
        }

        private void Editing(string currentText)
        {
            _oldEditText = _searchText;
            _searchText = currentText;
        }

        private void EndEdit(string currentText)
        {
            if(_keepOldTextInField && !string.IsNullOrEmpty(_oldEditText))
            {
                _searchText = _oldEditText;
                _searchField.text = _oldEditText;
                _keepOldTextInField = false;
            }
        }

        public void Clear()
        {
            _searchField.text = string.Empty;
            _itemLibrary.Reset();
        }
    }

}
