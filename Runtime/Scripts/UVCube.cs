/*
 * Copyright (c) 2022 Emil Forslund
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using UnityEngine;

namespace se.his.geometry {
    [ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class UVCube : MonoBehaviour {
    
        [SerializeField, HideInInspector]
        private Material DefaultMaterial;
        
        private MeshFilter _filter;
        private Mesh _mesh;
    
        [Min(1)] public int subDivisions = 1;
    
    #if UNITY_EDITOR
        void Start() {
            if (Application.isPlaying) {
                Destroy(this);
                return;
            }
            
            if (!TryGetComponent<MeshRenderer>(out var meshRenderer)) {
                Debug.LogError("Could not find MeshRenderer-component on UVCube");
                return;
            }
            
            if (meshRenderer.sharedMaterial == null) {
                if (DefaultMaterial == null) {
                    Debug.LogError("Default material is null");
                }
                meshRenderer.sharedMaterial = DefaultMaterial;
            }
        }
    
        void OnEnable() {
            if (!TryGetComponent(out _filter)) {
                Debug.LogError("Could not find MeshFilter-component on UVCube");
                return;
            }
            
            
    
            if (_filter.sharedMesh == null) {
                _mesh = new Mesh{ name = "UV Cube" };
                _filter.sharedMesh = _mesh;
            } else {
                _mesh = _filter.sharedMesh;
            }
            _mesh.MarkDynamic();
        }
    
        void Update() {
            var builder = new MeshBuilder();
    
            var t = transform;
            var scale = t.localScale;
            
            for (var i = 0; i < subDivisions; i++) {
                for (var j = 0; j < subDivisions; j++) {
                    for (var k = 0; k < 6; k++) {
                        builder.VertexMatrix = 
                            Matrix4x4.Rotate(Quaternion.AngleAxis(90 * ((k / 4) + 2 * (k / 5)), Vector3.right)) *
                            Matrix4x4.Rotate(Quaternion.AngleAxis(90 * Mathf.Min(k, 4), Vector3.up)) *
                            Matrix4x4.Rotate(Quaternion.AngleAxis(90, Vector3.left)) *
                            Matrix4x4.Translate(new Vector3(-.5f, .5f, -.5f)) *
                            Matrix4x4.Scale(Vector3.one * (1f / subDivisions)) *
                            Matrix4x4.Translate(new Vector3(i, 0, j));
    
                        builder.TextureMatrix =
                            new Matrix4x4(
                                new Vector4(1, 0, 0, 0),
                                new Vector4(0, 0, 1, 0),
                                new Vector4(0, 1, 0, 0),
                                new Vector4(0, 0, 0, 1)
                            ) *
                            
                            Matrix4x4.Rotate(Quaternion.AngleAxis(90, Vector3.right)) *
                            Matrix4x4.Rotate(Quaternion.AngleAxis(90 * Mathf.Min(k, 4), Vector3.down)) *
                            Matrix4x4.Rotate(Quaternion.AngleAxis(90 * (k / 4 + 2 * (k / 5)), Vector3.left)) *
                            
                            Matrix4x4.Scale(new Vector3(scale.x, scale.y, scale.z)) * // actual transformation
                            
                            Matrix4x4.Rotate(Quaternion.AngleAxis(90 * (k / 4 + 2 * (k / 5)), Vector3.right)) *
                            Matrix4x4.Rotate(Quaternion.AngleAxis(90 * Mathf.Min(k, 4), Vector3.up)) *
                            Matrix4x4.Rotate(Quaternion.AngleAxis(90, Vector3.left)) *
                            
                            new Matrix4x4(
                                new Vector4(1, 0, 0, 0),
                                new Vector4(0, 0, 1, 0),
                                new Vector4(0, 1, 0, 0),
                                new Vector4(0, 0, 0, 1)
                            ) *
                            Matrix4x4.Translate(new Vector3(i, j, 0));
                        
                        var a = builder.AddVertex(new Vector3(0, 0, 0), Vector3.up, new Vector2(0, 0));
                        var b = builder.AddVertex(new Vector3(0, 0, 1), Vector3.up, new Vector2(0, 1));
                        var c = builder.AddVertex(new Vector3(1, 0, 1), Vector3.up, new Vector2(1, 1));
                        var d = builder.AddVertex(new Vector3(1, 0, 0), Vector3.up, new Vector2(1, 0));
                        
                        builder.AddQuad(a, b, c, d);
                    }
                }
            }
            
            builder.Build(_mesh);
        }
    #endif
    }
}
