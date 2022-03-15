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
using UnityEngine;

namespace se.his.geometry {
    [ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class UVMesh : MonoBehaviour {

        [Tooltip("Prefab that will be scanned for MeshFilter/MeshRenderer pairs to build the mesh and material list of this object")]
        [SerializeField] private GameObject Prefab;
        
        private MeshFilter _filter;
        private MeshRenderer _meshRenderer;
        private Mesh _mesh;
        
        #if UNITY_EDITOR
        void Start() {
            if (Application.isPlaying) {
                Destroy(this);
                return;
            }
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
            
            _mesh = new Mesh{ name = "UV Cube" };
            _mesh.MarkDynamic();
            _filter.sharedMesh = _mesh;
        }

        void Update() {
            if (Prefab == null) return;
            var result = new MeshBatcher();
            var loaded = MeshBatcher.LoadFrom(Prefab);
            
            var t = transform;
            var scale = t.localScale;
            
            result.Add(loaded, Matrix4x4.identity, tri => {
                var n = (tri.V0.Normal + tri.V1.Normal + tri.V2.Normal) / 3.0f;
                
                if (Mathf.Abs(n.x) > Mathf.Abs(n.y)) {
                    if (Mathf.Abs(n.x) > Mathf.Abs(n.z)) {
                        return 
                            Matrix4x4.Translate(new Vector3(.5f, .5f, 0)) *
                            Matrix4x4.Scale(new Vector3(scale.z, scale.y, 1)) * 
                            Matrix4x4.Translate(new Vector3(-.5f, -.5f, 0));
                    } else { // XY
                        return Matrix4x4.Translate(new Vector3(.5f, .5f, 0)) *
                               Matrix4x4.Scale(new Vector3(scale.x, scale.y, 1)) * 
                               Matrix4x4.Translate(new Vector3(-.5f, -.5f, 0));
                    }
                } else {
                    if (Mathf.Abs(n.y) > Mathf.Abs(n.z)) {
                        return Matrix4x4.Translate(new Vector3(.5f, .5f, 0)) *
                               Matrix4x4.Scale(new Vector3(scale.x, scale.z, 1)) * 
                               Matrix4x4.Translate(new Vector3(-.5f, -.5f, 0));
                    } else { // XY
                        return Matrix4x4.Translate(new Vector3(.5f, .5f, 0)) *
                               Matrix4x4.Scale(new Vector3(scale.x, scale.y, 1)) * 
                               Matrix4x4.Translate(new Vector3(-.5f, -.5f, 0));
                    }
                }
            });

            var materials = new List<Material>();
            result.Build(_mesh, materials);
            _meshRenderer.sharedMaterials = materials.ToArray();
        }
        
        #endif
    }
}