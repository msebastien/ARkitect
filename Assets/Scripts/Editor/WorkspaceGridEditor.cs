using UnityEditor;

using ARKitect.Core;

namespace ARKitect.Editor
{
    [UnityEditor.CustomEditor(typeof(WorkspaceGrid))]
    public class WorkspaceGridEditor : UnityEditor.Editor
    {
        SerializedProperty _lineCount;
        SerializedProperty _cellSubdivisionCount;
        SerializedProperty _cellSize;

        private void OnEnable()
        {
            _lineCount = serializedObject.FindProperty("_lineCount");
            _cellSubdivisionCount = serializedObject.FindProperty("_cellSubdivisionCount");
            _cellSize = serializedObject.FindProperty("_cellSize");
        }

        public override void OnInspectorGUI()
        {
            WorkspaceGrid script = target as WorkspaceGrid;
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            script.LineCount = EditorGUILayout.IntSlider($"{_lineCount.displayName}", _lineCount.intValue, 1, WorkspaceGrid.MaxLines);
            script.CellSubdivisionCount = EditorGUILayout.IntSlider($"{_cellSubdivisionCount.displayName}", _cellSubdivisionCount.intValue, 1, WorkspaceGrid.MaxCellSubdivisions);
            script.CellSize = EditorGUILayout.Slider($"{_cellSize.displayName}", _cellSize.floatValue, 0.1f, 1.0f);

            EditorGUILayout.EndVertical();
        }
    }

}
