using System;
using System.Windows.Controls;

namespace WPFEditor
{
    /// <summary>
    /// Interface for adding a canvas to windows from other forms
    /// </summary>
    interface IAddable
    {
        /// <summary>
        /// Adds the given canvas to the window
        /// </summary>
        /// <param name="canvas">canvas to add</param>
        void Add(Canvas canvas);
    }
}
