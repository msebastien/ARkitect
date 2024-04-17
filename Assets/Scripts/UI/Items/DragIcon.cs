using ARKitect.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARKitect.UI.Items
{
    public class DragIcon : Singleton<DragIcon>
    {
        [SerializeField]
        private Image icon;

        private void Awake()
        {
            icon = GetComponent<Image>();
        }

        public void SetIcon(Sprite sprite)
        {
            icon.sprite = sprite;
            icon.color = Color.white;
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            icon.sprite = null;
            icon.color = Color.clear;
            gameObject.SetActive(false);
        }
    }

}