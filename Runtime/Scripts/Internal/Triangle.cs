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
    public readonly struct Triangle {
        public readonly Vertex V0, V1, V2;
        
        public Triangle(Vertex v0, Vertex v1, Vertex v2) {
            V0 = v0;
            V1 = v1;
            V2 = v2;
        }

        /// <summary>
        /// Returns a vector that is orthogonal to the plane of this triangle
        /// and that faces the same direction as the triangle is visible from.
        /// It is not based on the normal vectors of the vertices.
        /// </summary>
        public Vector3 Normal => Vector3.Cross(
            V1.Position - V0.Position,
            V2.Position - V0.Position
        );

        /// <summary>
        /// If this triangle faces away from the specified normal vector, return
        /// a copy of this with the winding order reversed.
        /// </summary>
        /// <param name="normal">the normal</param>
        /// <returns>a copy of this triangle</returns>
        public Triangle EnsureWindingOrderMatches(in Vector3 normal) {
            return Vector3.Dot(Normal, normal) >= 0.0f 
                ? this : new Triangle(V0, V2, V1);
        }
    }
}