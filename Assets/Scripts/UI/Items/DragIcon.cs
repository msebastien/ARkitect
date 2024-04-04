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

        public void SetSprite(Sprite sprite) { icon.sprite = sprite; }
        public void SetColor(Color color) { icon.color = color; }
    }

}