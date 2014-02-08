using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TowerHaven.AI
{
    /// <summary>
    /// A* Pathfinding Algorithm
    /// By: Steven Sucy
    /// </summary>
    public class Pathfinder
    {
        /// <summary>
        /// Directional coordinate offsets
        /// </summary>
        private short[,] directions = new short[,] { { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } };

        /// <summary>
        /// Map Positions List
        /// </summary>
        private SimpleList<SimpleList<Position>> mapPositions;

        /// <summary>
        /// List of open positions
        /// </summary>
        private ArrayList<Position> open;

        /// <summary>
        /// List of closed positions
        /// </summary>
        private ArrayList<Position> closed;

        /// <summary>
        /// The map traveling in
        /// </summary>
        private _2DMap map;

        /// <summary>
        /// Randomizer for avaiable positions
        /// </summary>
        private Random random;

        /// <summary>
        /// Starting position
        /// </summary>
        private Position start;

        /// <summary>
        /// Target position
        /// </summary>
        private Position target;

        /// <summary>
        /// Position currently checking
        /// </summary>
        private Position check;

        /// <summary>
        /// Sets the map, given a proper map
        /// </summary>
        public _2DMap Map
        {
            set
            {
                // Validate the map
                if (value == null)
                    throw new ArgumentNullException("Map cannot be null");
                else if (value.HorizontalSize <= 0 || value.VerticalSize <= 0)
                    throw new ArgumentOutOfRangeException("Map dimensions cannot be outside the set of natural numbers.");

                // Assign the map
                map = value;

                // Set up the position matrix according to the new map
                mapPositions = new SimpleList<SimpleList<Position>>();
                for (int i = 0; i < map.HorizontalSize; ++i)
                {
                    SimpleList<Position> row = new SimpleList<Position>();
                    mapPositions.Add(row);
                    for (int j = 0; j < map.VerticalSize; ++j)
                        row.Add(new Position(i, j));
                }

                // Set up the open and closed lists to the maximum capacity they could use
                open = new ArrayList<Position>(map.HorizontalSize * map.VerticalSize);
                closed = new ArrayList<Position>(map.HorizontalSize * map.VerticalSize);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Pathfinder()
        {
            random = new Random();
            start = new Position();
            target = new Position();
            check = new Position();
        }

        /// <summary>
        /// Constructor with a map
        /// </summary>
        /// <param name="map">_2DMap object</param>
        public Pathfinder(_2DMap map)
            : this()
        {
            this.Map = map;
        }

        /// <summary>
        /// Finds a near-optimal path from the starting location to the target location
        /// </summary>
        /// <param name="startLocation">start location</param>
        /// <param name="targetLocation">target location</param>
        /// <returns>List of moves</returns>
        public int[][] GetMoves(int[] startLocation, int[] targetLocation)
        {
            // Initialize the variables
            start.Location = startLocation;
            target.Location = targetLocation;
            return GetMoves(-1, -1);
        }
        /// <summary>
        /// Finds a near-optimal path from the starting location to the target location
        /// </summary>
        /// <param name="startLocation">start location</param>
        /// <param name="targetLocation">target location</param>
        /// <param name="excludeX">horizontal coordinate to ignore</param>
        /// <param name="excludeY">vertical coordinate to ignore</param>
        /// <returns>List of Moves</returns>
        public int[][] GetMoves(int[] startLocation, int[] targetLocation, int excludeX, int excludeY)
        {
            start.Location = startLocation;
            target.Location = targetLocation;
            return GetMoves(excludeX, excludeY);
        }

        /// <summary>
        /// Finds a near-optimal path from the starting location to the target location
        /// </summary>
        /// <param name="excludeX">horizontal coordinate to ignore</param>
        /// <param name="excludeY">vertical coordinate to ignore</param>
        /// <returns>List of Moves</returns>
        private int[][] GetMoves(int excludeX, int excludeY)
        {
            if (excludeX == start.X && excludeY == start.Y)
                throw new PathNotFoundException("Excluded tile was the starting point");
            open.Clear();
            closed.Clear();

            Position currentPosition;

            // Add the initial position to the open list
            open.Add(start);

            // Find a path to the destination
            do
            {
                // Locate the current best position in the open list
                currentPosition = null;
                int leastF = -1;
                foreach (Position p in open)
                {
                    int f = Math.Abs(p.X - target.X) + Math.Abs(p.Y - target.Y) + p.G;
                    if (currentPosition == null)
                    {
                        currentPosition = p;
                        leastF = f;
                    }
                    else if (leastF > f)
                    {
                        currentPosition = p;
                        leastF = f;
                    }
                }

                // Move the position to the closed list
                open.Remove(currentPosition);
                closed.Add(currentPosition);

                // Check the surrounding positions

                for (short x = 0; x < 8; ++x)
                {
                    check.X = currentPosition.X + directions[x, 0];
                    check.Y = currentPosition.Y + directions[x, 1];

                    // Ignore it if it is out of bounds, not traversible, or already in the closed list
                    if (check.X < 0
                        || check.Y < 0
                        || check.X >= map.HorizontalSize
                        || check.Y >= map.VerticalSize)
                        continue;
                    else if (!map.IsWalkable(check.X, check.Y)
                        || !map.IsWalkable(check.X, currentPosition.Y)
                        || !map.IsWalkable(currentPosition.X, check.Y)
                        || closed.Contains(check))
                        continue;
                    else if (check.X == excludeX && check.Y == excludeY)
                        continue;

                    // If it is in the open list, check if it was a better path than the current one
                    else if (open.Contains(check))
                    {
                        foreach (Position p in open)
                            if (p.Equals(check))
                                if (p.G < currentPosition.G - 1)
                                    currentPosition.Parent = p;
                    }

                    // Otherwise, add it to the open list
                    else
                    {
                        Position position = mapPositions[check.X][check.Y];
                        position.Parent = currentPosition;
                        open.Add(position);
                    }
                }
            }
            // Repeat while the end has not been reached and there are positions left to check
            while (!closed.Contains(target) && !open.Empty());

            // If the end was not found, throw an exception
            if (!closed.Contains(target))
                throw new PathNotFoundException("No path exists between the given points");

            // Connect the target location to the path
            target.Parent = currentPosition;

            // Backtrack while adding the moves to the move stack
            List<int[]> moves = new List<int[]>();
            Position back = target;
            do
            {
                moves.Add(back.Location);
                back = back.Parent;
            }
            while (!back.Equals(start));

            // Return the stack of moves
            return moves.ToArray();
        }
    }
}
