using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Phezu.Util {

    public class ListBuckets<T> where T : class {

        private Dictionary<int, List<T>> m_Buckets;

        public int Count => m_Buckets.Count;

        public ListBuckets() {
            m_Buckets = new();
        }

        public void Add(T item, int index) {
            m_Buckets.TryGetValue(index, out var bucket);

            if (bucket == null) {
                m_Buckets.Add(index, new());
                bucket = m_Buckets[index];
            }

            if (bucket.Contains(item))
                return;

            bucket.Add(item);
        }

        public void Remove(T item, int index) {
            m_Buckets.TryGetValue(index, out var bucket);

            if (bucket == null)
                return;
            if (!bucket.Contains(item))
                return;

            bucket.Remove(item);
        }

        public void Remove(T item) {
            foreach (var pair in m_Buckets) {
                foreach (var bucketItem in pair.Value) {
                    if (bucketItem == item) {
                        var bucket = m_Buckets[pair.Key];
                        bucket.Remove(item);
                        return;
                    }
                }
            }
        }

        public List<T> Get(int index) {
            m_Buckets.TryGetValue(index, out var bucket);

            return bucket;
        }

        public void Clear() {
            m_Buckets.Clear();
        }

        public List<KeyValuePair<int, List<T>>> GetBuckets() {
            return m_Buckets.ToList();
        }
    }
}
