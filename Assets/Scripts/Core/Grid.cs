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
        private const int maxLines = 30;
        private const int maxCellSubdivisions = 100;

        // Note: We need to draw n-1 lines for n subdivisions. For 10 cell subdivisions, 9 lines will be drawn.
        private const int maxVertexCount = maxLines * 2 * (maxCellSubdivisions - 1) * 2;

        /// <summary>
        /// Number of lines on each axis for the grid
        /// </summary>
        [SerializeField]
        [Range(1, maxLines)]
        private int lineCount = 20;

        /// <summary>
        /// Number of subdivisions for each cell on each axis
        /// </summary>
        [SerializeField]
        [Range(1, maxCellSubdivisions)]
        private int cellSubdivisionCount = 10; // 10 subdivisions per cell (For a 1m cell, a subdivision is 10 cm)

        /// <summary>
        /// Grid cell size, spacing between lines
        /// </summary>
        [SerializeField]
        [Range(0.1f, 1.0f)]
        private float cellSize = 1f; // 1m cell

        private float SubdivisionScale => cellSize / cellSubdivisionCount;

        /// <summary>
        /// Line vertices: one line (2 vertices) on each of the 2 axes of the grid
        /// </summary>
        private Vector3[] vertices = new Vector3[maxVertexCount]; // line vertices (2 vertices per line)

        /// <summary>
        /// Normal vectors
        /// </summary>
        private Vector3[] normals = new Vector3[maxVertexCount];

        /// <summary>
        /// UV coordinates
        /// </summary>
        private Vector2[] uv = new Vector2[maxVertexCount];

        /// <summary>
        /// Grid lines vertex indices
        /// </summary>
        private int[] gridIndices = new int[maxVertexCount];

        /// <summary>
        /// Subdivision lines vertex indices
        /// </summary>
        private int[] subdivisionIndices = new int[maxVertexCount];


        private void Start()
        {
            GetComponent<MeshFilter>().sharedMesh = GridMesh(lineCount, cellSize);
            transform.position = Vector3.zero;

            GetComponent<BoxCollider>().size = new Vector3(lineCount * cellSize, 0, lineCount * cellSize);
            GetComponent<BoxCollider>().isTrigger = true;
        }

        /// <summary>
        /// Create a grid mesh
        /// </summary>
        /// <param name="lineCount">Number of lines on each axis of the grid</param>
        /// <param name="scale">Line spacing, grid cell size</param>
        /// <returns>Grid Mesh</returns>
        private Mesh GridMesh(int lineCount, float scale)
        {
            if (lineCount == 0) return null;

            float gridHalf = (lineCount / 2f) * scale;

            lineCount++; // for adding missing grid borders (one border line on each axis)

            int vertexIdx = 0;
            int subdivisionGridIdx = 1;

            for (int line = 0; line < lineCount; line++)
            {
                // Main Grid lines
                // Line parallel to the Z Axis (vertices symmetrical with respect to the X axis)
                gridIndices[vertexIdx] = vertexIdx;
                uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                vertices[vertexIdx++] = new Vector3(line * scale - gridHalf, 0f, -gridHalf);

                gridIndices[vertexIdx] = vertexIdx;
                uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                vertices[vertexIdx++] = new Vector3(line * scale - gridHalf, 0f, gridHalf);

                // Line parallel to the X Axis (vertices symmetrical with respect to the Z axis)
                gridIndices[vertexIdx] = vertexIdx;
                uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                vertices[vertexIdx++] = new Vector3(-gridHalf, 0f, line * scale - gridHalf);

                gridIndices[vertexIdx] = vertexIdx;
                uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                vertices[vertexIdx++] = new Vector3(gridHalf, 0f, line * scale - gridHalf);

                // Subdivisions
                if (line >= lineCount - 1) continue; // Don't draw cell subdivisions beyond grid borders

                for (int subdivisionCellIdx = 1; subdivisionCellIdx < cellSubdivisionCount; subdivisionCellIdx++)
                {
                    // Subdivision parallel to the Z Axis (vertices symmetrical with respect to the X axis)
                    subdivisionIndices[vertexIdx] = vertexIdx;
                    uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                    vertices[vertexIdx++] = new Vector3(subdivisionGridIdx * SubdivisionScale - gridHalf, 0f, -gridHalf);

                    subdivisionIndices[vertexIdx] = vertexIdx;
                    uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                    vertices[vertexIdx++] = new Vector3(subdivisionGridIdx * SubdivisionScale - gridHalf, 0f, gridHalf);

                    // Subdivision parallel to the X Axis (vertices symmetrical with respect to the Z axis)
                    subdivisionIndices[vertexIdx] = vertexIdx;
                    uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                    vertices[vertexIdx++] = new Vector3(-gridHalf, 0f, subdivisionGridIdx * SubdivisionScale - gridHalf);

                    subdivisionIndices[vertexIdx] = vertexIdx;
                    uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                    vertices[vertexIdx++] = new Vector3(gridHalf, 0f, subdivisionGridIdx * SubdivisionScale - gridHalf);

                    Logger.LogInfo($"Line index= {line} / Subdivision Cell Index= {subdivisionCellIdx} / Subdivision Grid Index= {subdivisionGridIdx}");

                    subdivisionGridIdx++;
                }

                subdivisionGridIdx++; // To avoid drawing again over the main grid line, so we skip it.
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                normals[i] = Vector3.up;
            }

            Mesh tm = new Mesh();

            tm.name = "GridMesh";
            tm.vertices = vertices;
            tm.normals = normals;
            tm.subMeshCount = 2;
            tm.SetIndices(gridIndices, MeshTopology.Lines, 0);
            tm.SetIndices(subdivisionIndices, MeshTopology.Lines, 1);
            tm.uv = uv;

            return tm;
        }

    }

}
