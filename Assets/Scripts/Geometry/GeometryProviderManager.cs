using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using ARKitect.Geometry.Providers.ProBuilder;

namespace ARKitect.Geometry
{
    public enum MeshProviderType 
    {
        UnityMesh, ProBuilder
    }

    [AddComponentMenu("ARkitect/Geometry Provider Manager")]
    public class GeometryProviderManager : MonoBehaviour
    {
        [SerializeField]
        private MeshProviderType _providerType = MeshProviderType.ProBuilder;

        public void InitProvider(GameObject gameObject)
        {
            switch(_providerType)
            {
                case MeshProviderType.ProBuilder:
                    gameObject.AddComponent<ProBuilderGeometryProvider>().Init(); break;

                case MeshProviderType.UnityMesh:
                    throw new NotImplementedException("UnityMesh geometry provider is not implemented.");

                default:
                    break;
            }
            
        }

    }

}
