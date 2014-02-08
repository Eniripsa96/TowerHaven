using System;

namespace TowerHaven.AI
{
    /// <summary>
    /// A position in the path finding algorithm
    /// </summary>
    class Position
    {
        /// <summary>
        /// Position leading to this position
        /// </summary>
        private Position parent;

        /// <summary>
        /// Location of the position
        /// </summary>
        private int[] location;

        /// <summary>
        /// Number of steps from the starting point
        /// </summary>
        private int g;

        public Position Parent
        {
            get { return parent; }
            set 
            { 
                parent = value;
                g = parent.g + 1;
            }
        }

        public int[] Location
        {
            get { return location; }
            set { location = value; }
        }

        public int X
        {
            get { return location[0]; }
            set { location[0] = value; }
        }

        public int Y
        {
            get { return location[1]; }
            set { location[1] = value; }
        }

        public int G
        {
            get { return g; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        /// <param name="g">number of steps from the starting point</param>
        /// <param name="parent">position leading to this position (default = null)</param>
        public Position(int x, int y, Position parent)
        {
            location = new int[] { x, y };
            this.parent = parent;
            g = parent.g + 1;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        public Position(int x, int y)
        {
            location = new int[] { x, y };
            g = 0;
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public Position()
        {
            this.location = new int[2];
        }

        /// <summary>
        /// Equals override
        /// Makes two locations at the same location equal to eachother
        /// </summary>
        /// <param name="obj">location being compared to</param>
        /// <returns>true if they have the same location, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is Position)
            {
                Position p = (Position)obj;
                return (X == p.X && Y == p.Y);
            }
            else
                throw new InvalidCastException("Cannot convert " + obj.GetType().ToString() + " to Position");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
