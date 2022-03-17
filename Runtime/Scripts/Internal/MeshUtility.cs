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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace se.his.geometry {
    public static class MeshUtility {
        public static IEnumerable<(Transform transform, Mesh mesh, Material[] materials)> FindMeshes(GameObject asset) {
            foreach (var renderer in asset.GetComponentsInChildren<MeshRenderer>()) {
                if (!renderer.TryGetComponent<MeshFilter>(out var filter)) {
                    continue;
                }
                
                var mesh = filter.sharedMesh;
                if (mesh == null) {
                    Debug.LogError("Shared mesh is null");
                    continue;
                }
                
                if (!mesh.isReadable) {
                    Debug.LogError($"Could not read geometry data from mesh '{mesh.name}'");
                    continue;
                }
                
                var t = renderer.transform;
                var materials = renderer.sharedMaterials;
                
                var subMeshes = mesh.subMeshCount;
                if (subMeshes != materials.Length) {
                    Debug.LogError($"Mesh '{mesh.name}' has {subMeshes} " +
                                   "subMeshes, yet the number of materials " +
                                   "in the associated renderer is " +
                                   $"{materials.Length}.");
                    continue;
                }

                yield return (t, mesh, materials);
            }
        }

        public static void CopyMesh(Mesh src, Mesh dest) {
            dest.Clear();
            dest.vertices  = src.vertices;
            dest.normals   = src.normals;
            dest.uv        = src.uv;
            dest.triangles = src.triangles;
            dest.subMeshCount = src.subMeshCount;
            for (int i = 0; i < src.subMeshCount; i++) {
                dest.SetSubMesh(i, src.GetSubMesh(i));
            }
            dest.bounds = src.bounds;
        }
    }
}