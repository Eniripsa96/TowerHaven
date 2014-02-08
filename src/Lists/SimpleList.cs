using System;
using System.Collections;
using System.Collections.Generic;

namespace TowerHaven
{
    /// <summary>
    /// A simple Linked-List class
    /// </summary>
    /// <typeparam name="T">Type of the list</typeparam>
    public class SimpleList<T> : IEnumerable<T>
    {
        /// <summary>
        /// Start of the list
        /// </summary>
        private SimpleNode<T> front;

        /// <summary>
        /// End of the list
        /// </summary>
        private SimpleNode<T> end;

        /// <summary>
        /// Size of the list
        /// </summary>
        private int size;

        /// <summary>
        /// Property for the size
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// Returns the last element in the list
        /// </summary>
        public T Last
        {
            get
            {
                if (end == null)
                    throw new NullReferenceException("The list is empty!");
                return end.Data;
            }
        }

        /// <summary>
        /// Returns the top element fromm the list as if a stack (for sorting)
        /// </summary>
        public T Peek
        {
            get { return front.Data; }
        }

        /// <summary>
        /// Pops the top element from the list as if a stack (for sorting)
        /// </summary>
        public T Pop
        {
            get
            {
                size--;
                T data = front.Data;
                front = front.Next;
                return data;
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="x">index</param>
        /// <returns>element at x</returns>
        public T this[int x]
        {
            get 
            {
                if (x < 0)
                    throw new IndexOutOfRangeException("Index must be non-negative. Index was: " + x);
                SimpleNode<T> node = front;
                for (int i = 0; i < x; ++i)
                    node = node.Next;
                return node.Data;
            }
            set
            {
                if (x < 0)
                    throw new IndexOutOfRangeException("Index must be non-negative. Index was: " + x);
                SimpleNode<T> node = front;
                for (int i = 0; i < x; ++i)
                    node = node.Next;
                node.Data = value;
            }
        }

        /// <summary>
        /// Appends the element to the end of the list
        /// </summary>
        /// <param name="data">element</param>
        public void Add(T data)
        {
            if (Empty())
            {
                front = new SimpleNode<T>(data);
                end = front;
            }
            else
            {
                end.Next = new SimpleNode<T>(data);
                end = end.Next;
            }
            size++;
        }

        /// <summary>
        /// Removes the element from the list if it exists
        /// </summary>
        /// <param name="data">element</param>
        public void Remove(T data)
        {
            if (Empty())
                return;
            else if (front.Data.Equals(data))
            {
                front = front.Next;
                size--;
            }
            else
            {
                SimpleNode<T> previous = front;
                SimpleNode<T> current = front;
                while (current.Next != null)
                {
                    current = current.Next;
                    if (current.Data.Equals(data))
                    {
                        if (current == end)
                            end = previous;
                        previous.Next = current.Next;
                        size--;
                        break;
                    }
                    previous = previous.Next;
                }
            }
        }

        /// <summary>
        /// Empty method
        /// </summary>
        /// <returns>list is empty</returns>
        public Boolean Empty()
        {
            return size == 0;
        }

        /// <summary>
        /// Checks if the element is contained within the list
        /// </summary>
        /// <param name="data">element</param>
        /// <returns>element is in the list</returns>
        public Boolean Contains(T data)
        {
            foreach (T piece in this)
                if (data.Equals(piece))
                    return true;
            return false;
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            front = null;
            size = 0;
        }

        /// <summary>
        /// Returns the list as an array
        /// </summary>
        /// <returns>array</returns>
        public T[] ToArray()
        {
            T[] array = new T[size];
            int index = 0;
            foreach (T data in this)
                array[index++] = data;
            return array;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>list</returns>
        public override string ToString()
        {
            string s = "{";
            foreach (T data in this)
                s += data.ToString() + ",";
            return s.Substring(0, s.Length - 1) + "}";
        }

        /// <summary>
        /// Inserts a new node at the given index
        /// </summary>
        /// <param name="data">node data</param>
        /// <param name="index">node index</param>
        public void Insert(T data, int index)
        {
            SimpleNode<T> temp = front;
            if (index == 0)
            {
                front = new SimpleNode<T>(data);
                front.Next = temp;
            }
            for (int x = 0; x < index - 1; ++x)
                temp = temp.Next;
            SimpleNode<T> temp2 = temp.Next;
            temp.Next = new SimpleNode<T>(data);
            temp.Next.Next = temp2;
            size++;
        }

        /// <summary>
        /// Enumeration method
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var node = front;
            while (node != null)
            {
                yield return node.Data;
                node = node.Next;
            }
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