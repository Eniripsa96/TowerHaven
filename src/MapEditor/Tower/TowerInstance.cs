using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFEditor
{
    public class TowerInstance
    {
        /// <summary>
        /// Tower name
        /// </summary>
        string name;

        /// <summary>
        /// Horizontal coordinate
        /// </summary>
        int horizontalPosition;

        /// <summary>
        /// Vertical coordinate
        /// </summary>
        int verticalPosition;

        /// <summary>
        /// Frames since fired last
        /// </summary>
        public int ticks;

        /// <summary>
        /// Name property
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Horizontal position property
        /// </summary>
        public int HorizontalPosition
        {
            get { return horizontalPosition; }
        }

        /// <summary>
        /// Vertical Position property
        /// </summary>
        public int VerticalPosition
        {
            get { return verticalPosition; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        public TowerInstance(string name, int x, int y)
        {
            this.name = name;
            this.horizontalPosition = x;
            this.verticalPosition = y;
        }

        /// <summary>
        /// Returns distance to the given point
        /// </summary>
        /// <param name="location">location of target</param>
        /// <returns>distance</returns>
        public int GetDistance(int x, int y)
        {
            return (int)Math.Sqrt((x - horizontalPosition) * (x - horizontalPosition) + (y - verticalPosition) * (y - verticalPosition));
        }
    }
}
