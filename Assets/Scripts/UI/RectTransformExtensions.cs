using UnityEngine;

namespace ARKitect.UI
{
    internal static class RectTransformExtensions
    {
        public static void FillParent(this RectTransform self, RectTransform parent)
        {
            self.SetParent(parent);
            self.localPosition = Vector3.zero;
            self.anchorMin = Vector2.zero;
            self.anchorMax = Vector2.one;
            self.offsetMin = Vector2.zero;
            self.offsetMax = Vector2.zero;
            self.pivot = new Vector2(0.5f, 0.5f);
            self.rotation = Quaternion.identity;
            self.localScale = Vector3.one;
        }

        public static void FillParent(this RectTransform self, RectTransform parent, Vector2 pivot)
        {
            self.SetParent(parent);
            self.localPosition = Vector3.zero;
            self.anchorMin = Vector2.zero;
            self.anchorMax = Vector2.one;
            self.offsetMin = Vector2.zero;
            self.offsetMax = Vector2.zero;
            self.pivot = pivot;
            self.rotation = Quaternion.identity;
            self.localScale = Vector3.one;
        }
    }

}