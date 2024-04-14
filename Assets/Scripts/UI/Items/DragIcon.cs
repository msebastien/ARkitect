using ARKitect.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARKitect.UI.Items
{
    public class DragIcon : Singleton<DragIcon>
    {
        private Image icon;

        private void Awake()
        {
            icon = GetComponent<Image>();
        }

        private void SetSprite(Sprite sprite) { icon.sprite = sprite; }
        private void SetColor(Color color) { icon.color = color; }

        public void SetIcon(Sprite icon)
        {
            SetSprite(icon);
            SetColor(Color.white);
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            SetSprite(null);
            SetColor(Color.clear);
            gameObject.SetActive(false);
        }
    }

}