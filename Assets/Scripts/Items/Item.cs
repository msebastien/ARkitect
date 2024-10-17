using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using ARKitect.Items.Resource;

namespace ARKitect.Items
{
    public enum ItemCategory
    {
        Misc,
        Prop,
        Material,
        Foundation,
        Wall
    }

    public enum ItemType
    {
        Object,
        Material
    }

    /// <summary>
    /// Define properties of an item from the building parts' library
    /// </summary>
    public class Item
    {
        [PreviewField]
        [SerializeField]
        private Sprite icon;
        public Sprite Icon => icon;

        [SerializeField]
        private string name = "default";
        public string Name => name;

        [SerializeField]
        private ItemType type;
        public ItemType Type => type;

        [SerializeField]
        private ItemCategory category;
        public ItemCategory Category => category;

        [SerializeField]
        private string description;
        public string Description => description;

        [SerializeField]
        private string author;
        public string Author => author;

        [SerializeField]
        private bool markedAsFavorite = false;
        public bool MarkedAsFavorite
        {
            get => markedAsFavorite;
            set => markedAsFavorite = value;
        }

        private List<string> tags = new List<string>();
        public List<string> Tags => tags;

        [SerializeField]
        private bool builtin = false;
        public bool IsBuiltIn => builtin;

        [SerializeField]
        private IResource resource;
        public IResource Resource => resource;

        // TODO: Refactor to avoid this extremely big constructor using an ItemBuilder
        public Item(string name, Sprite icon, IResource resource, ItemCategory category = ItemCategory.Misc, string description = "", string author = "", IEnumerable<string> tags = default, bool builtin = false)
        {
            this.name = name;
            this.icon = icon;
            this.category = category;
            this.resource = resource;
            this.description = description;
            this.author = author;
            this.tags.AddRange(tags);
            this.builtin = builtin;

            if (resource is ResourceObject)
                this.type = ItemType.Object;
            else if (resource is ResourceMaterial)
                this.type = ItemType.Material;
        }

    }

}