using Sirenix.OdinInspector;
using UnityEngine;

namespace ARKitect.Items
{
    public enum ItemCategory
    {
        Foundation,
        Wall,
        Prop,
        Misc
    }

    /// <summary>
    /// Define properties for an item from the building parts' library
    /// </summary>
    public class Item
    {
        [PreviewField]
        [SerializeField]
        private Sprite icon;
        [SerializeField]
        private string name = "default";
        [SerializeField]
        private ItemCategory category;
        [SerializeField]
        private string description;
        [SerializeField]
        private GameObject prefab;
        

        public Sprite Icon => icon;
        public string Name => name;
        public ItemCategory Category => category;
        public string Description => description;

        public Item() { }

        public Item(string name, Sprite icon, GameObject prefab, ItemCategory category = ItemCategory.Misc, string description = default)
        {
            this.name = name;
            this.icon = icon;
            this.category = category;
            this.prefab = prefab;
            this.description = description;
        }
    }

}