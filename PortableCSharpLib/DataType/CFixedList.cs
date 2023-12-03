using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableCSharpLib.DataType
{
    /// <summary>
    /// List with fixed length
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CFixedList<T> : IEnumerable<T>
    {
        static CFixedList() { General.CheckDateTime(); }

        public int CurrentPosition { get; private set; }
        public int Count { get { return q.Count; } }
        public T CurrentElement { get { return q.ElementAt(this.CurrentPosition); } }
        private List<T> q;
        private int Limit { get; set; }

        public CFixedList(int limit)
        {
            this.Limit = limit;
            this.q = new List<T>();
            this.CurrentPosition = -1;
        }
        public CFixedList(CFixedList<T> inQueue) : this(inQueue, inQueue.Limit) { }
        public CFixedList(IEnumerable<T> inQueue, int limit)
        {
            this.Limit = limit;
            this.CurrentPosition = -1;
            this.q = new List<T>(inQueue);
        }
        public T this[int index] { get { return q[index]; } set { lock (this) { q[index] = value; } } }

        public bool MoveNext()
        {
            if (CurrentPosition <= q.Count - 1)
                ++CurrentPosition;

            if (CurrentPosition <= q.Count - 1)
                return true;
            else
                return false;                     //pass the end of the queue
        }
        public void MovePrev()
        {
            if (CurrentPosition >= 0)
                --CurrentPosition;
        }
        public bool Reset()
        {
            CurrentPosition = -1;
            return true;
        }
        public void MoveToBegin()
        {
            CurrentPosition = -1;
        }
        public void MoveToEnd()
        {
            CurrentPosition = q.Count;
        }
        public void Clear()
        {
            this.q = new List<T>();
            this.CurrentPosition = -1;
        }
        public void AddElement(T command)
        {
            q.Add(command);
            lock (this) {
                while (q.Count > Limit)
                    q.RemoveAt(0);
            }
        }
        public void RemoveElement()
        {
            lock (this) {
                q.RemoveAt(0);
            }
        }
        public T PrevElement()
        {
            if (CurrentPosition >= 0)
                --CurrentPosition;

            if (CurrentPosition >= 0)
                return this.CurrentElement;
            else
                return default(T);
        }
        public T NextElement()
        {
            if (CurrentPosition <= q.Count - 1)
                ++CurrentPosition;

            if (CurrentPosition <= q.Count - 1)
                return this.CurrentElement;
            else
                return default(T);
        }

        public IEnumerator<T> GetEnumerator()
        {
            CurrentPosition = 0;
            while (CurrentPosition < Count) {
                yield return CurrentElement;
                ++CurrentPosition;
            }
            //return new CFixedListEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
