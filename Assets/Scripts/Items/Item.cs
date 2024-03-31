using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ARKitect.Items
{
    public enum ItemCategory
    {
        Foundation,
        Wall,
        Texture,
        Prop,
        Misc
    }

    public interface IItem 
    {
        public Sprite Icon { get; }
        public string Name { get; }
        public ItemCategory Category { get; }
        public string Description { get; }
    }

    /// <summary>
    /// Define properties of an item from the building parts' library
    /// </summary>
    public class Item<T> : IItem where T : UnityEngine.Object
    {
        [PreviewField]
        [SerializeField]
        protected Sprite icon;
        [SerializeField]
        protected string name = "default";
        [SerializeField]
        protected ItemCategory category;
        [SerializeField]
        protected string description;

        [SerializeField]
        private T resource; // T -> GameObject or Texture2D
        // Link an item to a command (Command Pattern) ?

        public Sprite Icon => icon;
        public string Name => name;
        public ItemCategory Category => category;
        public string Description => description;

        public T Resource => resource;

        public Item(string name, Sprite icon, T resource, ItemCategory category = ItemCategory.Misc, string description = default)
        {
            this.name = name;
            this.icon = icon;
            this.category = category;
            this.resource = resource;
            this.description = description;
        }

    }

}