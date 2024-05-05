using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

using ARKitect.Core;
using ARKitect.Items;

namespace ARKitect.UI.Items
{
    /// <summary>
    /// Allows to display Item metadata in the UI 
    /// </summary>
    [AddComponentMenu("ARkitect/UI/Item Info")]
    public class UIItemInfo : MonoBehaviour
    {
        [Header("Item")]
        [SerializeField]
        [ReadOnly]
        private string _itemId = "";
        public string ItemId
        {
            get
            {
                return _itemId;
            }
            set
            {
                if (_itemId != value)
                {
                    _itemId = value;
                    Reset();
                    UpdateItemInfo();
                }
            }
        }

        [Header("UI Fields")]
        [SerializeField]
        private TextMeshProUGUI _nameText;

        [SerializeField]
        private TextMeshProUGUI _authorText;

        [SerializeField]
        private TextMeshProUGUI _typeText;

        [SerializeField]
        private Image _iconImage;

        [Header("Defaults")]
        [SerializeField]
        private string _defaultNameText = "[Name]";

        [SerializeField]
        private string _defaultAuthorText = "By [Author]";

        [SerializeField]
        private string _defaultTypeText = "[Type]";

        [Header("Placeholders")]
        [SerializeField]
        private string _placeholderName = "[Name]";

        [SerializeField]
        private string _placeholderAuthor = "[Author]";

        [SerializeField]
        private string _placeholderType = "[Type]";


        private void UpdateItemInfo()
        {
            var item = PrefabsManager.Items[new Identifier(_itemId)];

            if (_nameText != null)
                _nameText.text = _nameText.text.Replace(_placeholderName, item.Name);

            if (_authorText != null)
                _authorText.text = _authorText.text.Replace(_placeholderAuthor, item.Author);

            if (_typeText != null)
                _typeText.text = _typeText.text.Replace(_placeholderType, item.Type.ToString());

            if (_iconImage != null)
            {
                _iconImage.color = Color.white;
                _iconImage.sprite = item.Icon;
            }
        }

        private void Reset()
        {
            if (_nameText != null)
                _nameText.text = _defaultNameText;

            if (_authorText != null)
                _authorText.text = _defaultAuthorText;

            if (_typeText != null)
                _typeText.text = _defaultTypeText;

            if (_iconImage != null)
            {
                _iconImage.color = Color.clear;
                _iconImage.sprite = null;
            }
        }
    }

}
