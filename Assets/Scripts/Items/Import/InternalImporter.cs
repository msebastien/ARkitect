using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

using Logger = ARKitect.Core.Logger;
using ARKitect.Core;

namespace ARKitect.Items.Import
{
    class JsonObject
    {
        [JsonProperty("item")]
        public ItemData Item { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    class ItemData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string IconPath { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemType Type { get; set; }

        [JsonProperty("category")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemCategory Category { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        private List<ItemResourceData> _resources = new List<ItemResourceData>();

        [JsonProperty("resources")]
        public List<ItemResourceData> Resources
        {
            get
            {
                if (_resources == null)
                {
                    Logger.LogError("InternalImporter: List of Item Resources is null");
                    _resources = new List<ItemResourceData>();
                }
                return _resources;
            }
            set { _resources = value; }
        }
    }

    class ItemResourceData
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
    }

    /// <summary>
    /// This importer is used to serialize and load built-in items and prefabs
    /// </summary>
    [AddComponentMenu("ARkitect/Internal Importer")]
    public class InternalImporter : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Path to the directory which contains all the item definition JSON files")]
        private string _path = "Items";

        /// <summary>
        /// Import built-in item definitions synchronously from the internal "Resources" directory
        /// </summary>
        public void Import()
        {
            Dictionary<string, IItem> items = ParseItemDefinitionJSON();
            foreach (var item in items)
            {
                PrefabsManager.Instance.Items.Add(new Identifier(item.Key), item.Value);
            }
        }

        // TODO: Maybe, we should load assets asynchronously ?
        private Dictionary<string, IItem> ParseItemDefinitionJSON()
        {
            Dictionary<string, IItem> parsedItems = new Dictionary<string, IItem>();

            foreach (TextAsset itemDefFile in Resources.LoadAll<TextAsset>(_path))
            {
                // Deserialize text into JSON object
                var parsedJson = JsonConvert.DeserializeObject<JsonObject>(itemDefFile.text, new JsonSerializerSettings
                {
                    Error = OnParsingError
                });
                var parsedItemData = parsedJson.Item;

                // Create an Item resource based on the deserialized data
                UnityEngine.Object itemResource = null;
                foreach (var resourceDef in parsedItemData.Resources)
                {
                    string path = resourceDef.Path.Split('.')[0];
                    string resourceType = resourceDef.Type.ToLower();

                    switch(parsedItemData.Type) 
                    {
                        case ItemType.Object:
                            if(resourceType == "prefab") itemResource = Resources.Load<GameObject>(path);
                            break;
                        case ItemType.Material:
                            if(resourceType == "material") itemResource = Resources.Load<Material>(path);
                            break;
                        default:
                            break;
                    }

                }

                // Create Item icon
                Sprite icon = Resources.Load<Sprite>(parsedItemData.IconPath.Split(".")[0]);

                if (itemResource == null) continue; // Go to the next item

                // Create item and cast resource into the correct Unity type
                var type = itemResource.GetType();
                if (type == typeof(GameObject))
                {
                    Item<GameObject> item = new Item<GameObject>(parsedItemData.Name, icon,
                                                                parsedItemData.Type,
                                                                (GameObject)itemResource,
                                                                parsedItemData.Category,
                                                                parsedItemData.Description,
                                                                parsedItemData.Author,
                                                                true);
                    parsedItems.Add(parsedItemData.Id, item);
                }
                else if (type == typeof(Material))
                {
                    Item<Material> item = new Item<Material>(parsedItemData.Name, icon,
                                                                parsedItemData.Type,
                                                                (Material)itemResource,
                                                                parsedItemData.Category,
                                                                parsedItemData.Description,
                                                                parsedItemData.Author,
                                                                true);
                    parsedItems.Add(parsedItemData.Id, item);
                }
            }

            return parsedItems;
        }

        private void OnParsingError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
            Logger.LogError(args.ErrorContext.Error.Message);
            args.ErrorContext.Handled = true;
        }
    }

}