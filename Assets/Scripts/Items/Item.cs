using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ARKitect.Items
{
    public enum ItemCategory
    {
        Misc,
        Prop,
        Texture,
        Foundation,
        Wall
    }

    public enum ItemType
    {
        Object,
        Material
    }

    public interface IItem 
    {
        public Sprite Icon { get; }
        public string Name { get; }
        public ItemType Type { get; }
        public ItemCategory Category { get; }
        public string Description { get; }
        public string Author { get; }
        public bool MarkedAsFavorite { get; set; }
        public bool IsBuiltIn { get; }
    }

    /// <summary>
    /// Define properties of an item from the building parts' library
    /// </summary>
    public class Item<T> : IItem where T : UnityEngine.Object
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

        [SerializeField]
        private bool builtin = false;
        public bool IsBuiltIn => builtin;

        [SerializeField]
        private T resource; // T -> GameObject or Texture2D
        // Link an item to a command (Command Pattern) ?    
        public T Resource => resource;

        public Item(string name, Sprite icon, ItemType type, T resource, ItemCategory category = ItemCategory.Misc, string description = "", string author = "", bool builtin = false)
        {
            this.name = name;
            this.icon = icon;
            this.type = type;
            this.category = category;
            this.resource = resource;
            this.description = description;
            this.author = author;
            this.builtin = builtin;
        }

    }

}