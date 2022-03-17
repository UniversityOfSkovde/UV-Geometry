using System.Linq;
using UnityEditor;
using UnityEngine;

namespace se.his.geometry {
    [CustomEditor(typeof(UVMesh)), CanEditMultipleObjects]
    public class UVMeshEditor : Editor {
        
        private void OnEnable() {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private void OnDisable() {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (targets.OfType<UVMesh>().Any(m => !m.RegenerateAutomatically)) {
                EditorGUILayout.Space();
                if (GUILayout.Button("Regenerate Geometry")) {
                    UpdateAllTargeted(true);
                }
            }
        }

        private void OnHierarchyChanged() {
            UpdateAllTargeted(false);
        }

        private void UpdateAllTargeted(bool recordUpdate) {
            var selected = targets.OfType<UVMesh>().Cast<Object>().ToArray();
            if (recordUpdate) {
                Undo.RecordObjects(selected, "Regenerate geometry");
            }
            
            foreach (var t in selected) {
                if (t is UVMesh uvMesh) {
                    uvMesh.RegenerateGeometry();
                    if (recordUpdate) EditorUtility.SetDirty(uvMesh);
                }
            }
        }
    }
}