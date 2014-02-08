using System;
using System.Collections.Generic;
using System.Collections;

namespace TowerHaven.AI 
{
    /// <summary>
    /// Array implementation of a variable-length list
    /// </summary>
    class ArrayList<T> : IEnumerable<T>
    {
        /// <summary>
        /// Content of the list
        /// </summary>
        T[] content;

        /// <summary>
        /// Index of the start of the active list
        /// </summary>
        int first;

        /// <summary>
        /// Index of the end of the active list
        /// </summary>
        int last;

        /// <summary>
        /// Size of active elemnts
        /// </summary>
        int size;

        /// <summary>
        /// Indexer property
        /// </summary>
        /// <param name="x">index</param>
        /// <returns>element at x</returns>
        public T this[int x]
        {
            get { return content[(first + x) % content.Length]; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">The capacity of the list</param>
        public ArrayList(int capacity)
        {
            Clear();
            content = new T[capacity + 1];
        }

        /// <summary>
        /// Adds a position to the list
        /// </summary>
        /// <param name="p">position to add</param>
        public void Add(T p)
        {
            last = (last + 1) % content.Length;
            content[last] = p;
            size++;
        }

        /// <summary>
        /// Removes the position from the list
        /// </summary>
        /// <param name="p">position to remove</param>
        public void Remove(T p)
        {
            for (int i = 0; i < size; ++i)
                if (content[(first + i) % content.Length].Equals(p))
                {
                    for (int j = first + i; j > first; --j)
                        content[j % content.Length] = content[(j - 1 + content.Length) % content.Length];
                    size--;
                    first++;
                    return;
                }
        }

        /// <summary>
        /// Checks whether or not the position is within the list
        /// </summary>
        /// <param name="p"></param>
        /// <returns>true if it is contained, false otherwise</returns>
        public bool Contains(T p)
        {
            foreach (T element in this)
                if (element.Equals(p))
                    return true;

            return false;
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            first = 0;
            last = -1;
            size = 0;
        }

        /// <summary>
        /// Returns whether or not the list is empty
        /// </summary>
        /// <returns>true if empty, false otherwise</returns>
        public bool Empty()
        {
            return size == 0;
        }

        /// <summary>
        /// Enumeration method
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = first; i < first + size; ++i)
                yield return content[i % content.Length];
        }

        /// <summary>
        /// Enumeration accessor
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
