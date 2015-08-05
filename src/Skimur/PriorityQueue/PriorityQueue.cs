using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.PriorityQueue
{
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        private List<T> data;

        public PriorityQueue()
        {
            this.data = new List<T>();
        }

        public void Enqueue(T item)
        {
            if (data.Count == 0)
            {
                data.Add(item);
                return;
            }

            int insertAt = -1;
            for (var dataIndex = 0; dataIndex < data.Count; dataIndex++)
            {
                if (data[dataIndex].CompareTo(item) >= 0)
                {
                    insertAt = dataIndex;
                    break;
                }
            }

            if (insertAt == -1)
            {
                data.Add(item);
                return;
            }

            data.Insert(insertAt, item);
        }

        public T Dequeue()
        {
            var item = data[data.Count - 1];
            data.RemoveAt(data.Count - 1);
            return item;
        }

        public T Peek()
        {
            return data[data.Count - 1];
        }

        public int Count
        {
            get { return data.Count; }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    } // PriorityQueue
}
