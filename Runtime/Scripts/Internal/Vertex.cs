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
using UnityEngine;

namespace se.his.geometry {
    public readonly struct Vertex : IEquatable<Vertex> {

        public readonly Vector3 Position;
        public readonly Vector3 Normal;
        public readonly Vector2 TexCoord;

        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord) {
            Position = position;
            Normal   = normal;
            TexCoord = texCoord;
        }

        public static Vertex Lerp(Vertex from, Vertex to, float t) {
            return new Vertex(
                Vector3.Lerp(from.Position, to.Position, t),
                Vector3.Slerp(from.Normal, to.Normal, t),
                Vector2.Lerp(from.TexCoord, to.TexCoord, t)
            );
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ Normal.GetHashCode();
                hashCode = (hashCode * 397) ^ TexCoord.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(Vertex other) {
            return Position.Equals(other.Position) 
                && Normal.Equals(other.Normal) 
                && TexCoord.Equals(other.TexCoord);
        }

        public override bool Equals(object obj) {
            return obj is Vertex other && Equals(other);
        }
    }
}