using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARKitect.Core
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("ARkitect/Grid")]
    public class Grid : MonoBehaviour
    {
        public int lines = 20;
        public float scale = 1f;

        private void Start()
        {
            GetComponent<MeshFilter>().sharedMesh = GridMesh(lines, scale);
            transform.position = Vector3.zero;

            GetComponent<BoxCollider>().size = new Vector3(lines * scale, 0, lines * scale);
            GetComponent<BoxCollider>().isTrigger = true;
        }

        Mesh GridMesh(int lineCount, float scale)
        {
            float half = (lineCount / 2f) * scale;

            lineCount++; // for grid borders

            Vector3[] lines = new Vector3[lineCount * 4]; // line vertices (4 vertices per line)
            Vector3[] normals = new Vector3[lineCount * 4];
            Vector2[] uv = new Vector2[lineCount * 4];
            int[] indices = new int[lineCount * 4]; // vertex indices

            int n = 0;
            for (int y = 0; y < lineCount; y++)
            {
                indices[n] = n;
                uv[n] = y % 10 == 0 ? Vector2.one : Vector2.zero;
                lines[n++] = new Vector3(y * scale - half, 0f, -half);

                indices[n] = n;
                uv[n] = y % 10 == 0 ? Vector2.one : Vector2.zero;
                lines[n++] = new Vector3(y * scale - half, 0f, half);

                indices[n] = n;
                uv[n] = y % 10 == 0 ? Vector2.one : Vector2.zero;
                lines[n++] = new Vector3(-half, 0f, y * scale - half);

                indices[n] = n;
                uv[n] = y % 10 == 0 ? Vector2.one : Vector2.zero;
                lines[n++] = new Vector3(half, 0f, y * scale - half);
            }

            for (int i = 0; i < lines.Length; i++)
            {
                normals[i] = Vector3.up;
            }

            Mesh tm = new Mesh();

            tm.name = "GridMesh";
            tm.vertices = lines;
            tm.normals = normals;
            tm.subMeshCount = 1;
            tm.SetIndices(indices, MeshTopology.Lines, 0);
            tm.uv = uv;

            return tm;
        }
    }

}
