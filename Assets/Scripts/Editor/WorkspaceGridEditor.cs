using UnityEditor;

using ARKitect.Core;

namespace ARKitect.Editor
{
    [UnityEditor.CustomEditor(typeof(WorkspaceGrid))]
    public class WorkspaceGridEditor : UnityEditor.Editor
    {
        SerializedProperty _cellCount;
        SerializedProperty _cellSubdivisionCount;
        SerializedProperty _cellSize;

        private void OnEnable()
        {
            _cellCount = serializedObject.FindProperty("_cellCount");
            _cellSubdivisionCount = serializedObject.FindProperty("_cellSubdivisionCount");
            _cellSize = serializedObject.FindProperty("_cellSize");
        }

        public override void OnInspectorGUI()
        {
            WorkspaceGrid script = target as WorkspaceGrid;
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            script.CellCount = EditorGUILayout.IntSlider($"{_cellCount.displayName}", _cellCount.intValue, 1, WorkspaceGrid.MaxCells);
            script.CellSubdivisionCount = EditorGUILayout.IntSlider($"{_cellSubdivisionCount.displayName}", _cellSubdivisionCount.intValue, 1, WorkspaceGrid.MaxCellSubdivisions);
            script.CellSize = EditorGUILayout.Slider($"{_cellSize.displayName}", _cellSize.floatValue, 0.1f, 1.0f);

            EditorGUILayout.EndVertical();
        }
    }

}
