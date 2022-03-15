using System;
using System.Collections.Generic;
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
    public readonly struct Plane : IEquatable<Plane> {
        private readonly Vector3 Normal;
        private readonly float Distance;

        public Plane(Vector3 normal, float distance) {
            Normal   = normal;
            Distance = distance;
        }

        /// <summary>
        /// Returns the signed distance from a point to the plane. The sign will
        /// be positive if the point is considered on top of the plane and
        /// negative if the point is considered below the plane.
        /// </summary>
        /// <param name="v">the vertex to check</param>
        /// <returns>the signed distance</returns>
        public float SignedDistance(in Vertex v) {
            return SignedDistance(in v.Position);
        }
        
        /// <summary>
        /// Returns the signed distance from a point to the plane. The sign will
        /// be positive if the point is considered on top of the plane and
        /// negative if the point is considered below the plane.
        /// </summary>
        /// <param name="p">the point to check</param>
        /// <returns>the signed distance</returns>
        public float SignedDistance(in Vector3 p) {
            return Vector3.Dot(Normal, p) - Distance;
        }

        public void CutTriangles(List<Triangle> tris) {
            for (var k = 0; k < tris.Count;) {
                var t = tris[k];

                // The intersection between a triangle
                // and a plane is a line given by two
                // points l0 and l1.
                switch (Intersects(in t, out var t0, out Triangle t1)) {
                    case TriangleIntersection.AllVerticesAhead:
                        // The triangle is completely inside. Keep it in the list.
                        k++;
                        break;
                    case TriangleIntersection.TwoVerticesAhead:
                        // One vertex is outside, so generate a quad from the
                        // intersection line and the remaining two vertices.
                        tris[k] = t0; // Replace current
                        tris.Insert(k + 1, t1); // Insert after it
                        k += 2;
                        break;
                    case TriangleIntersection.OneVertexAhead:
                        // Only one vertex was inside. Form a new triangle
                        // from the intersection line and this one vertex.
                        tris[k] = t0;
                        k++;
                        break;
                    case TriangleIntersection.NoVerticesAhead:
                        // Triangle was completely outside. Remove it.
                        tris.RemoveAt(k);
                        break; // Don't increment i
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public enum TriangleIntersection {
            AllVerticesAhead,
            TwoVerticesAhead,
            OneVertexAhead,
            NoVerticesAhead
        }

        public TriangleIntersection Intersects(
                in Triangle tri, 
                out Triangle a,
                out Triangle b) {
            
            var v0 = tri.V0;
            var v1 = tri.V1;
            var v2 = tri.V2;
            var d0 = SignedDistance(in v0);
            var d1 = SignedDistance(in v1);
            var d2 = SignedDistance(in v2);
            
            // Sort the vertices so that v2 refers to the most positive one and
            // v0 to the least positive one. This might change the winding
            // order, which we will compensate for later.
            if (d0 > d1) (d0, d1, v0, v1) = (d1, d0, v1, v0); // Flip 0 and 1
            if (d1 > d2) {
                (d1, d2, v1, v2) = (d2, d1, v2, v1); // Flip 1 and 2
                if (d0 > d1) (d0, d1, v0, v1) = (d1, d0, v1, v0); // Flip 0 and 1
            }
            
            // If all points are ahead of the plane:
            if (d0 >= -float.Epsilon) {
                a = default; b = default;
                return TriangleIntersection.AllVerticesAhead;
            }
            
            // If all points are behind the plane:
            if (d2 <= float.Epsilon) {
                a = default; b = default;
                return TriangleIntersection.NoVerticesAhead;
            }

            var n = tri.Normal;
            
            // d0 is definitely behind the plane and d2 is definitely ahead
            // check if d1 is also ahead
            Vertex l0, l1;
            if (d1 >= 0.0f) {
                // The result will be a quad (2 triangles) since two vertices
                // are ahead of the plane and the intersection line consists of
                // two points.
                l0 = Vertex.Lerp(v0, v1, -d0 / (d1 - d0));
                l1 = Vertex.Lerp(v0, v2, -d0 / (d2 - d0));
                a = new Triangle(l0, v1, v2).EnsureWindingOrderMatches(in n);
                b = new Triangle(l0, v2, l1).EnsureWindingOrderMatches(in n);
                return TriangleIntersection.TwoVerticesAhead;
            }
            
            // d0 is the only vertex that is ahead of the plane. Compute the two
            // intersection points based on v2 and return one triangle.
            l0 = Vertex.Lerp(v2, v0, d2 / (d2 - d0));
            l1 = Vertex.Lerp(v2, v1, d2 / (d2 - d1));
            a = new Triangle(l0, l1, v2).EnsureWindingOrderMatches(in n);
            b = default;
            return TriangleIntersection.TwoVerticesAhead;
        }
        
        public bool Equals(Plane other) {
            return Normal.Equals(other.Normal) && Distance.Equals(other.Distance);
        }

        public override bool Equals(object obj) {
            return obj is Plane other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return (Normal.GetHashCode() * 397) ^ Distance.GetHashCode();
            }
        }
    }
}