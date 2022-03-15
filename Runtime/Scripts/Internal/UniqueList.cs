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

namespace se.his.geometry {
    public sealed class UniqueList<T> {
        private readonly Dictionary<T, int> indices;
        private readonly List<T> values;

        public UniqueList() {
            indices = new Dictionary<T, int>();
            values = new List<T>();
        }

        public UniqueList(UniqueList<T> prototype) {
            indices = new Dictionary<T, int>(prototype.indices);
            values = new List<T>(prototype.values);
        }

        public int Count => values.Count;

        public T this[int idx] => values[idx];

        public int AddOrFind(T element) {
            if (!indices.TryGetValue(element, out var index)) {
                index = values.Count;
                values.Add(element);
            }

            return index;
        }
    }
}