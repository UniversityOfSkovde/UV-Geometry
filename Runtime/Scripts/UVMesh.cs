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

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace se.his.geometry {
    [ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class UVMesh : MonoBehaviour {

        [Tooltip("Prefab that will be scanned for MeshFilter/MeshRenderer pairs to build the mesh and material list of this object")]
        [SerializeField] private GameObject Prefab;

        [Tooltip("Should textures stretch or should they have seams")]
        [SerializeField] private bool StretchTextures;
        
        [Tooltip("Regenerate every frame or only when the button is pressed")]
        [SerializeField] public bool RegenerateAutomatically;
        
        private MeshFilter _filter;
        private MeshRenderer _meshRenderer;
        private Mesh _mesh;

        [NonSerialized] private bool _requireUpdate = true;
        [NonSerialized] private Vector3 _previousScale = Vector3.zero;
        [SerializeField, HideInInspector] private Vector3 _originalSize;
        [SerializeField, HideInInspector] private Vector3 _originalOffset;

#if UNITY_EDITOR
        void Start() {
            if (Application.isPlaying) {
                Destroy(this);
                return;
            }

            _previousScale = transform.localScale;
        }

        void OnEnable() {
            if (Application.isPlaying) {
                return;
            }
            
            if (!TryGetComponent(out _filter)) {
                Debug.LogError("Could not find MeshFilter-component on UVMesh");
                return;
            }
            
            if (!TryGetComponent(out _meshRenderer)) {
                Debug.LogError("Could not find MeshRenderer-component on UVMesh");
                return;
            }
            
            _mesh = new Mesh{ name = "UV Mesh" };
            _mesh.indexFormat = IndexFormat.UInt32;
            _mesh.MarkDynamic();
            _filter.sharedMesh = _mesh;
        }

        private void OnValidate() {
            if (RegenerateAutomatically) {
                _requireUpdate = true;
            }
        }

        void Update() {
            if (Prefab == null) return;
            if (_requireUpdate || (RegenerateAutomatically && transform.localScale != _previousScale)) {
                RegenerateGeometry();
            }
        }

        private const float LineThickness = 2.0f;
        private void OnDrawGizmosSelected() {
            Handles.color = Color.gray;
            
            var t = transform;
            var p = t.position + t.rotation * _originalOffset;
            var s = 0.5f * Vector3.Scale(_originalSize, t.localScale);
            var x = t.right * s.x;
            var y = t.up * s.y;
            var z = t.forward * s.z;
            
            DrawLines(new [] {
                p - x - y - z,
                p + x - y - z,
                p + x - y + z,
                p - x - y + z
            });
            
            DrawLines(new [] {
                p - x + y - z,
                p + x + y - z,
                p + x + y + z,
                p - x + y + z
            });
            
            Handles.DrawLine(p - x - y - z, p - x + y - z, LineThickness);
            Handles.DrawLine(p + x - y - z, p + x + y - z, LineThickness);
            Handles.DrawLine(p + x - y + z, p + x + y + z, LineThickness);
            Handles.DrawLine(p - x - y + z, p - x + y + z, LineThickness);
        }

        private void DrawLines(Vector3[] lines) {
            for (var i = 0; i < lines.Length; i++) {
                Handles.DrawLine(lines[i], lines[(i + 1) % lines.Length], LineThickness);
            }
        }

        public void RegenerateGeometry() {
            if (Prefab == null) return;
            _requireUpdate = false;
            
            var result = new MeshBatcher();
            var loaded = MeshBatcher.LoadFrom(Prefab);
            
            var t = transform;
            var scale = t.localScale;
            var (min, max) = loaded.Bounds;
            var tileSize = max - min;
            _originalSize = tileSize;
            _originalOffset = min + 0.5f * tileSize;
            
            var tileSizeInv = new Vector3(
                1.0f / tileSize.x,
                1.0f / tileSize.y,
                1.0f / tileSize.z
            );
            
            var tileCount = new Vector3Int(
                Math.Max(1, Mathf.RoundToInt(scale.x)),
                Math.Max(1, Mathf.RoundToInt(scale.y)),
                Math.Max(1, Mathf.RoundToInt(scale.z))
            );
            var tileCountInv = new Vector3(
                1.0f / tileCount.x,
                1.0f / tileCount.y,
                1.0f / tileCount.z
            );

            var actualScale = new Vector3(
                scale.x / tileCount.x,
                scale.y / tileCount.y,
                scale.z / tileCount.z
            );

            for (var i = 0; i < tileCount.x; i++) {
                for (var j = 0; j < tileCount.y; j++) {
                    for (var k = 0; k < tileCount.z; k++) {
                        var vertexMatrix =
                            Matrix4x4.Scale(tileCountInv) *
                            Matrix4x4.Translate(min) *
                            Matrix4x4.Scale(tileSize) *
                            Matrix4x4.Translate(new Vector3(i, j, k) - 0.5f * (Vector3) (tileCount - Vector3Int.one)) *
                            Matrix4x4.Scale(tileSizeInv) *
                            Matrix4x4.Translate(-min);
                        
                        result.Add(loaded, vertexMatrix, tri => {
                            var n = (tri.V0.Normal + tri.V1.Normal + tri.V2.Normal) / 3.0f;
                
                            if (StretchTextures)
                                return Matrix4x4.identity;
                            
                            if (Mathf.Abs(n.x) > Mathf.Abs(n.y)) {
                                
                                // YZ
                                if (Mathf.Abs(n.x) > Mathf.Abs(n.z)) { 
                                    return 
                                        Matrix4x4.Translate(new Vector3(.5f, .5f, 0)) *
                                        Matrix4x4.Scale(new Vector3(actualScale.z, actualScale.y, 1)) * 
                                        Matrix4x4.Translate(new Vector3(-.5f, -.5f, 0));
                                }
                    
                                // XY
                                return Matrix4x4.Translate(-new Vector3(i, j, k) + 0.5f * (Vector3) (tileCount - Vector3Int.one)) *
                                       Matrix4x4.Scale(new Vector3(actualScale.x, actualScale.y, 1)) * 
                                       Matrix4x4.Translate(new Vector3(i, j, k) - 0.5f * (Vector3) (tileCount - Vector3Int.one));
                            }
                
                            // XZ
                            if (Mathf.Abs(n.y) > Mathf.Abs(n.z)) { 
                                return Matrix4x4.Translate(new Vector3(.5f, .5f, 0)) *
                                       Matrix4x4.Scale(new Vector3(actualScale.x, actualScale.z, 1)) * 
                                       Matrix4x4.Translate(new Vector3(-.5f, -.5f, 0));
                            }
                
                            // XY
                            return Matrix4x4.Translate(-new Vector3(i, j, k) + 0.5f * (Vector3) (tileCount - Vector3Int.one)) *
                                   Matrix4x4.Scale(new Vector3(actualScale.x, actualScale.y, 1)) * 
                                   Matrix4x4.Translate(new Vector3(i, j, k) - 0.5f * (Vector3) (tileCount - Vector3Int.one));
                        });
                    }
                }
            }

            var materials = new List<Material>();
            result.Build(_mesh, materials);
            _meshRenderer.sharedMaterials = materials.ToArray();
        }
        
        #endif
    }
}