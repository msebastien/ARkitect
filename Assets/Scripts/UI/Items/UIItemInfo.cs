using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

using ARKitect.Core;
using ARKitect.Items;

namespace ARKitect.UI.Items
{
    [AddComponentMenu("ARkitect/UI/Item Info")]
    public class UIItemInfo : MonoBehaviour
    {
        [Header("Item")]
        [SerializeField]
        [ReadOnly]
        private string _itemId;
        public string ItemId
        {
            get
            {
                return _itemId;
            }
            set
            {
                _itemIdChanged = true;
                _itemId = value;
            }
        }
        private bool _itemIdChanged = false;

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


        // Update is called once per frame
        void Update()
        {
            if (_itemIdChanged)
            {
                Reset();
                UpdateItemInfo();
                _itemIdChanged = false;
            }
        }

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
