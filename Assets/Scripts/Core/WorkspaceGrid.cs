using UnityEngine;

namespace ARKitect.Core
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("ARkitect/Workspace Grid")]
    public class WorkspaceGrid : MonoBehaviour
    {
        public const int MaxCells = 30;
        public const int MaxCellSubdivisions = 100;

        // Note: We need to draw n-1 lines for n subdivisions. For 10 cell subdivisions, 9 lines will be drawn.
        public const int MaxVertexCount = MaxCells * 2 * (MaxCellSubdivisions - 1) * 2;

        /// <summary>
        /// Number of cells (and lines to draw) on each axis for the grid
        /// </summary>
        [Range(1, MaxCells)]
        [SerializeField]
        private int _cellCount = 20;
        public int CellCount
        {
            get { return _cellCount; }
            set
            {
                if (value != _prevCellCount)
                {
                    _cellCount = Mathf.Clamp(value, 1, MaxCells);

                    DestroyMesh();
                    CreateNewGridMesh();

                    _prevCellCount = _cellCount;
                }
            }
        }
        private int _prevCellCount = 20;

        /// <summary>
        /// Number of subdivisions for each cell on each axis
        /// </summary>
        [Range(1, MaxCellSubdivisions)]
        [SerializeField]
        private int _cellSubdivisionCount = 10; // 10 subdivisions per cell (For a 1m cell, a subdivision is 10 cm)
        public int CellSubdivisionCount
        {
            get { return _cellSubdivisionCount; }
            set
            {
                if (value != _prevCellSubdivisionCount)
                {
                    _cellSubdivisionCount = Mathf.Clamp(value, 1, MaxCellSubdivisions);

                    DestroyMesh();
                    CreateNewGridMesh();

                    _prevCellSubdivisionCount = _cellSubdivisionCount;
                }
            }
        }
        private int _prevCellSubdivisionCount = 10;

        /// <summary>
        /// Grid cell size, spacing between lines
        /// </summary>
        [Range(0.1f, 1.0f)]
        [SerializeField]
        private float _cellSize = 1f; // 1m cell
        public float CellSize
        {
            get { return _cellSize; }
            set
            {
                if (value != _prevCellSize)
                {
                    _cellSize = Mathf.Clamp(value, 0.1f, 1.0f);

                    DestroyMesh();
                    CreateNewGridMesh();

                    _prevCellSize = _cellSize;
                }
            }
        }
        private float _prevCellSize = 1f;

        private float SubdivisionScale => _cellSize / _cellSubdivisionCount;


        private void Start()
        {
            CreateNewGridMesh();
            transform.position = Vector3.zero;
        }

        /// <summary>
        /// Create a grid mesh
        /// </summary>
        /// <param name="cellCount">Number of grid cells (= number of lines to draw) on each axis</param>
        /// <param name="cellSubdivisionCount">Number of subdivisions for each cell on each axis</param>
        /// <param name="cellSize">Line spacing, grid cell size</param>
        /// <returns>Grid Mesh</returns>
        private Mesh GridMesh(int cellCount, int cellSubdivisionCount, float cellSize)
        {
            if (cellCount == 0) return null;

            // Buffers
            // Line vertices: one line (2 vertices) on each of the 2 axes of the grid
            Vector3[] vertices = new Vector3[MaxVertexCount];
            // Normal vectors
            Vector3[] normals = new Vector3[MaxVertexCount];
            // UV Coordinates
            Vector2[] uv = new Vector2[MaxVertexCount];
            // Grid lines vertex indices
            int[] gridIndices = new int[MaxVertexCount];
            // Subdivision lines vertex indices
            int[] subdivisionIndices = new int[MaxVertexCount];
            // X Axis line vertex indices
            int[] XaxisIndices = new int[MaxVertexCount];
            // Z Axis line vertex indices
            int[] ZaxisIndices = new int[MaxVertexCount];

            float gridHalf = (cellCount / 2f) * cellSize;

            cellCount++; // for adding missing grid borders (one border line on each axis)
            int vertexIdx = 0;
            int subdivisionGridIdx = 1;
            for (int line = 0; line < cellCount; line++)
            {
                // Main Grid lines
                // Line parallel to the Z Axis (vertices symmetrical with respect to the X axis)
                // Line: First vertex
                if (line * cellSize == gridHalf) // Retrieve axes vertex indices to put them in a different submesh
                    ZaxisIndices[vertexIdx] = vertexIdx;
                else
                    gridIndices[vertexIdx] = vertexIdx;
                uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                vertices[vertexIdx++] = new Vector3(line * cellSize - gridHalf, 0f, -gridHalf);

                // Line: Second vertex
                if (line * cellSize == gridHalf)
                    ZaxisIndices[vertexIdx] = vertexIdx;
                else
                    gridIndices[vertexIdx] = vertexIdx;
                uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                vertices[vertexIdx++] = new Vector3(line * cellSize - gridHalf, 0f, gridHalf);

                // Line parallel to the X Axis (vertices symmetrical with respect to the Z axis)
                // Line: First vertex
                if (line * cellSize == gridHalf)
                    XaxisIndices[vertexIdx] = vertexIdx;
                else
                    gridIndices[vertexIdx] = vertexIdx;
                uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                vertices[vertexIdx++] = new Vector3(-gridHalf, 0f, line * cellSize - gridHalf);

                // Line: Second vertex
                if (line * cellSize == gridHalf)
                    XaxisIndices[vertexIdx] = vertexIdx;
                else
                    gridIndices[vertexIdx] = vertexIdx;
                uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                vertices[vertexIdx++] = new Vector3(gridHalf, 0f, line * cellSize - gridHalf);

                // Subdivisions
                if (line >= cellCount - 1) continue; // Don't draw cell subdivisions beyond grid borders

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
            tm.subMeshCount = 4;
            tm.SetIndices(gridIndices, MeshTopology.Lines, 0);
            tm.SetIndices(subdivisionIndices, MeshTopology.Lines, 1);
            tm.SetIndices(XaxisIndices, MeshTopology.Lines, 2);
            tm.SetIndices(ZaxisIndices, MeshTopology.Lines, 3);
            tm.uv = uv;

            return tm;
        }

        public void CreateNewGridMesh()
        {
            GetComponent<MeshFilter>().mesh = GridMesh(_cellCount, _cellSubdivisionCount, _cellSize);
            UpdateColliderSize();
        }

        public void DestroyMesh()
        {
#if UNITY_EDITOR
            DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);
#else
            Destroy(GetComponent<MeshFilter>().sharedMesh);
#endif
        }

        private void UpdateColliderSize()
        {
            GetComponent<BoxCollider>().size = new Vector3(_cellCount * _cellSize, 0, _cellCount * _cellSize);
            GetComponent<BoxCollider>().isTrigger = true;
        }


    }

}
