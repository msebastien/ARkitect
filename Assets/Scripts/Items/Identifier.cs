using System;
using UnityEngine;

namespace ARKitect.Items
{
    /// <summary>
    /// Define an identifier for items
    /// </summary>
    public class Identifier : IComparable<Identifier>, IEquatable<Identifier>
    {
        [SerializeField]
        private string namespaceId = "arkitect";
        [SerializeField]
        private string nameId = "undefined";

        public string Namespace => namespaceId;

        public string Name => nameId;

        public bool IsUndefined => nameId == "undefined";

        public Identifier() { }

        public Identifier(string namespaceId, string nameId)
        {
            Init(namespaceId, nameId);
        }

        public Identifier(string identifierStr)
        {
            string[] ids = identifierStr.Split(':');
            if (ids?.Length > 0)
            {
                Init(ids[0], ids[1]);
            }
            else
            {
                Init("", identifierStr);
            }        
        }

        private void Init(string namespaceId, string nameId) 
        {
            if (!String.IsNullOrWhiteSpace(namespaceId))
                this.namespaceId = namespaceId.ToLower().Replace(" ", "_");
            if (!String.IsNullOrWhiteSpace(nameId))
                this.nameId = nameId.ToLower().Replace(" ", "_");
        }

        public override string ToString()
        {
            return namespaceId + ":" + nameId;
        }

        public int CompareTo(Identifier other)
        {
            return ToString().CompareTo(other?.ToString());
        }

        public bool Equals(Identifier other)
        {
            return ToString() == other?.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is Identifier && this.Equals(obj as Identifier);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

}
