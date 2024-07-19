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
        public const int MaxLines = 30;
        public const int MaxCellSubdivisions = 100;

        // Note: We need to draw n-1 lines for n subdivisions. For 10 cell subdivisions, 9 lines will be drawn.
        public const int MaxVertexCount = MaxLines * 2 * (MaxCellSubdivisions - 1) * 2;

        /// <summary>
        /// Number of lines on each axis for the grid
        /// </summary>
        [Range(1, MaxLines)]
        [SerializeField]
        private int _lineCount = 20;
        public int LineCount
        {
            get { return _lineCount; }
            set
            {
                if (value != _prevLineCount)
                {
                    _lineCount = Mathf.Clamp(value, 1, MaxLines);
                    GetComponent<BoxCollider>().size = new Vector3(_lineCount * _cellSize, 0, _lineCount * _cellSize);
#if UNITY_EDITOR
                    DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);
#else
                    Destroy(GetComponent<MeshFilter>().sharedMesh);
#endif
                    GetComponent<MeshFilter>().mesh = GridMesh(_lineCount, _cellSubdivisionCount, _cellSize);
                    _prevLineCount = _lineCount;
                }
            }
        }
        private int _prevLineCount = 20;

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
#if UNITY_EDITOR
                    DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);
#else
                    Destroy(GetComponent<MeshFilter>().sharedMesh);
#endif
                    GetComponent<MeshFilter>().mesh = GridMesh(_lineCount, _cellSubdivisionCount, _cellSize);
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
                    GetComponent<BoxCollider>().size = new Vector3(_lineCount * _cellSize, 0, _lineCount * _cellSize);
#if UNITY_EDITOR
                    DestroyImmediate(GetComponent<MeshFilter>().sharedMesh);
#else
                    Destroy(GetComponent<MeshFilter>().sharedMesh);
#endif
                    GetComponent<MeshFilter>().mesh = GridMesh(_lineCount, _cellSubdivisionCount, _cellSize);
                    _prevCellSize = _cellSize;
                }
            }
        }
        private float _prevCellSize = 1f;

        private float SubdivisionScale => _cellSize / _cellSubdivisionCount;

        private void Start()
        {
            GetComponent<MeshFilter>().mesh = GridMesh(_lineCount, _cellSubdivisionCount, _cellSize);
            transform.position = Vector3.zero;

            GetComponent<BoxCollider>().size = new Vector3(_lineCount * _cellSize, 0, _lineCount * _cellSize);
            GetComponent<BoxCollider>().isTrigger = true;
        }

        /// <summary>
        /// Create a grid mesh
        /// </summary>
        /// <param name="lineCount">Number of lines on each axis of the grid</param>
        /// <param name="cellSubdivisionCount">Number of subdivisions for each cell on each axis</param>
        /// <param name="cellSize">Line spacing, grid cell size</param>
        /// <returns>Grid Mesh</returns>
        private Mesh GridMesh(int lineCount, int cellSubdivisionCount, float cellSize)
        {
            if (lineCount == 0) return null;

            // Buffers
            // Line vertices: one line (2 vertices) on each of the 2 axes of the grid
            Vector3[] _vertices = new Vector3[MaxVertexCount];
            // Normal vectors
            Vector3[] _normals = new Vector3[MaxVertexCount];
            // UV Coordinates
            Vector2[] _uv = new Vector2[MaxVertexCount];
            // Grid lines vertex indices
            int[] _gridIndices = new int[MaxVertexCount];
            // Subdivision lines vertex indices
            int[] _subdivisionIndices = new int[MaxVertexCount];
            // X Axis line vertex indices
            int[] _XaxisIndices = new int[MaxVertexCount];
            // Z Axis line vertex indices
            int[] _ZaxisIndices = new int[MaxVertexCount];

            float gridHalf = (lineCount / 2f) * cellSize;

            lineCount++; // for adding missing grid borders (one border line on each axis)
            int vertexIdx = 0;
            int subdivisionGridIdx = 1;
            for (int line = 0; line < lineCount; line++)
            {
                // Main Grid lines
                // Line parallel to the Z Axis (vertices symmetrical with respect to the X axis)
                // Line: First vertex
                if (line * cellSize == gridHalf) // Retrieve axes vertex indices to put them in a different submesh
                    _ZaxisIndices[vertexIdx] = vertexIdx;
                else
                    _gridIndices[vertexIdx] = vertexIdx;
                _uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                _vertices[vertexIdx++] = new Vector3(line * cellSize - gridHalf, 0f, -gridHalf);

                // Line: Second vertex
                if (line * cellSize == gridHalf)
                    _ZaxisIndices[vertexIdx] = vertexIdx;
                else
                    _gridIndices[vertexIdx] = vertexIdx;
                _uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                _vertices[vertexIdx++] = new Vector3(line * cellSize - gridHalf, 0f, gridHalf);

                // Line parallel to the X Axis (vertices symmetrical with respect to the Z axis)
                // Line: First vertex
                if (line * cellSize == gridHalf)
                    _XaxisIndices[vertexIdx] = vertexIdx;
                else
                    _gridIndices[vertexIdx] = vertexIdx;
                _uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                _vertices[vertexIdx++] = new Vector3(-gridHalf, 0f, line * cellSize - gridHalf);

                // Line: Second vertex
                if (line * cellSize == gridHalf)
                    _XaxisIndices[vertexIdx] = vertexIdx;
                else
                    _gridIndices[vertexIdx] = vertexIdx;
                _uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                _vertices[vertexIdx++] = new Vector3(gridHalf, 0f, line * cellSize - gridHalf);

                // Subdivisions
                if (line >= lineCount - 1) continue; // Don't draw cell subdivisions beyond grid borders

                for (int subdivisionCellIdx = 1; subdivisionCellIdx < cellSubdivisionCount; subdivisionCellIdx++)
                {
                    // Subdivision parallel to the Z Axis (vertices symmetrical with respect to the X axis)
                    _subdivisionIndices[vertexIdx] = vertexIdx;
                    _uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                    _vertices[vertexIdx++] = new Vector3(subdivisionGridIdx * SubdivisionScale - gridHalf, 0f, -gridHalf);

                    _subdivisionIndices[vertexIdx] = vertexIdx;
                    _uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                    _vertices[vertexIdx++] = new Vector3(subdivisionGridIdx * SubdivisionScale - gridHalf, 0f, gridHalf);

                    // Subdivision parallel to the X Axis (vertices symmetrical with respect to the Z axis)
                    _subdivisionIndices[vertexIdx] = vertexIdx;
                    _uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                    _vertices[vertexIdx++] = new Vector3(-gridHalf, 0f, subdivisionGridIdx * SubdivisionScale - gridHalf);

                    _subdivisionIndices[vertexIdx] = vertexIdx;
                    _uv[vertexIdx] = line % 10 == 0 ? Vector2.one : Vector2.zero;
                    _vertices[vertexIdx++] = new Vector3(gridHalf, 0f, subdivisionGridIdx * SubdivisionScale - gridHalf);

                    Logger.LogInfo($"Line index= {line} / Subdivision Cell Index= {subdivisionCellIdx} / Subdivision Grid Index= {subdivisionGridIdx}");

                    subdivisionGridIdx++;
                }

                subdivisionGridIdx++; // To avoid drawing again over the main grid line, so we skip it.
            }

            for (int i = 0; i < _vertices.Length; i++)
            {
                _normals[i] = Vector3.up;
            }

            Mesh tm = new Mesh();

            tm.name = "GridMesh";
            tm.vertices = _vertices;
            tm.normals = _normals;
            tm.subMeshCount = 4;
            tm.SetIndices(_gridIndices, MeshTopology.Lines, 0);
            tm.SetIndices(_subdivisionIndices, MeshTopology.Lines, 1);
            tm.SetIndices(_XaxisIndices, MeshTopology.Lines, 2);
            tm.SetIndices(_ZaxisIndices, MeshTopology.Lines, 3);
            tm.uv = _uv;

            return tm;
        }

    }

}
