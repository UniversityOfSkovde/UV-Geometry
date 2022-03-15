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

namespace se.his.geometry {
    public class MeshBuilder {
        private readonly List<Vector3> vertices = new();
        private readonly List<Vector3> normals = new();
        private readonly List<Vector2> uv = new();
        private readonly List<int> triangles = new();
    
        public Matrix4x4 VertexMatrix = Matrix4x4.identity;
        public Matrix4x4 TextureMatrix = Matrix4x4.identity;
    
        private readonly Dictionary<(Vector3, Vector3, Vector2), int> existing = new();
    
        public int AddVertex(Vector3 position, Vector3 normal, Vector2 uv) {
            var pos = VertexMatrix.MultiplyPoint(position);
            var nor = VertexMatrix.MultiplyVector(normal);
            var tex = TextureMatrix.MultiplyPoint(uv);
            var key = (pos, nor, tex);
            if (existing.TryGetValue(key, out var index)) {
                return index;
            }
            
            index = vertices.Count;
            vertices.Add(pos);
            normals.Add(nor);
            this.uv.Add(tex);
            return index;
        }
    
        public void AddQuad(
                int bottomLeft, int topLeft, 
                int topRight, int bottomRight) {
            // First triangle
            triangles.Add(bottomLeft);
            triangles.Add(topLeft);
            triangles.Add(topRight); 
            
            // Second triangle
            triangles.Add(bottomLeft);
            triangles.Add(topRight);
            triangles.Add(bottomRight);
        }
    
        public void Build(Mesh mesh) {
            mesh.Clear();
            mesh.vertices  = vertices.ToArray();
            mesh.normals   = normals.ToArray();
            mesh.uv        = uv.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateTangents();
            mesh.MarkModified();
        }
    }
}
