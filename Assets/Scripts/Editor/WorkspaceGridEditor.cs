using UnityEditor;

using ARKitect.Core;

namespace ARKitect.Editor
{
    [UnityEditor.CustomEditor(typeof(WorkspaceGrid))]
    public class WorkspaceGridEditor : UnityEditor.Editor
    {
        private SerializedProperty _cellCount;
        private SerializedProperty _cellSubdivisionCount;
        private SerializedProperty _cellSize;

        private SerializedProperty _gridMaterial;
        private SerializedProperty _cellSubdivisionMaterial;
        private SerializedProperty _XaxisMaterial;
        private SerializedProperty _ZaxisMaterial;

        private void OnEnable()
        {
            _cellCount = serializedObject.FindProperty("_cellCount");
            _cellSubdivisionCount = serializedObject.FindProperty("_cellSubdivisionCount");
            _cellSize = serializedObject.FindProperty("_cellSize");

            _gridMaterial = serializedObject.FindProperty("_gridMaterial");
            _cellSubdivisionMaterial = serializedObject.FindProperty("_cellSubdivisionMaterial");
            _XaxisMaterial = serializedObject.FindProperty("_XaxisMaterial");
            _ZaxisMaterial = serializedObject.FindProperty("_ZaxisMaterial");
        }

        public override void OnInspectorGUI()
        {
            WorkspaceGrid script = target as WorkspaceGrid;
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
            script.CellCount = EditorGUILayout.IntSlider($"{_cellCount.displayName}", _cellCount.intValue, 1, WorkspaceGrid.MaxCells);
            script.CellSubdivisionCount = EditorGUILayout.IntSlider($"{_cellSubdivisionCount.displayName}", _cellSubdivisionCount.intValue, 1, WorkspaceGrid.MaxCellSubdivisions);
            script.CellSize = EditorGUILayout.Slider($"{_cellSize.displayName}", _cellSize.floatValue, 0.1f, 1.0f);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_gridMaterial);
            EditorGUILayout.PropertyField(_cellSubdivisionMaterial);
            EditorGUILayout.PropertyField(_XaxisMaterial);
            EditorGUILayout.PropertyField(_ZaxisMaterial);

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }

}
