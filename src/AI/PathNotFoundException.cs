using System;

namespace TowerHaven.AI
{
    /// <summary>
    /// Custom exception for the pathfinding program
    /// </summary>
    class PathNotFoundException : ApplicationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="msg">error message</param>
        public PathNotFoundException(string msg)
            : base(msg)
        { }
    }
}
