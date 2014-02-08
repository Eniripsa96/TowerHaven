using System;

namespace TowerHaven.AI
{
    /// <summary>
    /// A node based implementation of a stack
    /// </summary>
    /// <typeparam name="T">The data type the NodeStack holds</typeparam>
    public class Stack<T>
    {
        /// <summary>
        /// A reference to the top element in the stack
        /// </summary>
        private StackNode<T> top;

        /// <summary>
        /// Size of stack
        /// </summary>
        private int size;

        /// <summary>
        /// Size property
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// Adds an element to the stack
        /// </summary>
        /// <param name="data">The element to add</param>
        public void push(T data)
        {
            if (empty())
                top = new StackNode<T>(data);
            else
            {
                StackNode<T> temp = top;
                top = new StackNode<T>(data);
                top.Next = temp;
            }
            size++;
        }

        /// <summary>
        /// Quietly removes the top element on the stack
        /// </summary>
        /// <exception>Throws an UnderflowException if the stack is empty</exception>
        public T pop()
        {
            if (empty())
                throw new Exception("Stack is empty!");
            T t = top.Data;
            top = top.Next;
            size--;
            return t;
        }

        /// <summary>
        /// Indicates whether the stack is empty or not
        /// </summary>
        /// <returns>True if the stack is empty, false otherwise</returns>
        public bool empty()
        {
            return size == 0;
        }

        /// <summary>
        /// Clears the stack
        /// </summary>
        public void clear()
        {
            top = null;
        }

        /// <summary>
        /// Adds a node to the stack
        /// </summary>
        /// <param name="node">node</param>
        private void Add(StackNode<T> node)
        {
            top = node;
        }

        /// <summary>
        /// Copies the stack
        /// </summary>
        /// <returns>stack</returns>
        public Stack<T> Copy()
        {
            Stack<T> newStack = new Stack<T>();
            newStack.Add(top.Clone());
            newStack.size = size;
            return newStack;
        }
    }
}
