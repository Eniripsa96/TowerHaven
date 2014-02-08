using System;
using System.Collections.Generic;

using TowerHaven;

namespace WPFEditor
{
    public class SpawnPoint
    {
        /// <summary>
        /// List of enemies in the wave
        /// </summary>
        public List<EnemyInstance> enemies;

        /// <summary>
        /// Horizontal coordinate of the starting position
        /// </summary>
        public int xStart;

        /// <summary>
        /// Vertical coordinate of the starting position
        /// </summary>
        public int yStart;

        /// <summary>
        /// Horizontal coordinate of the end location
        /// </summary>
        public int xEnd;

        /// <summary>
        /// Vertical coordinate of the end location
        /// </summary>
        public int yEnd;

        /// <summary>
        /// start position property
        /// </summary>
        public int[] Start
        {
            get { return new int[] { xStart, yStart }; }
        }

        /// <summary>
        /// End position property
        /// </summary>
        public int[] End
        {
            get { return new int[] { xEnd, yEnd }; }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SpawnPoint()
        {
            xStart = 0;
            yStart = 0;
            xEnd = 0;
            yEnd = 0;
            enemies = new List<EnemyInstance>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">spawn point data</param>
        public SpawnPoint(PairList<string, object> data)
        {
            enemies = new List<EnemyInstance>();
            foreach (PairListNode<string, object> element in data)
                if (element.Key.Equals("xStart"))
                    xStart = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("yStart"))
                    yStart = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("xEnd"))
                    xEnd = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("yEnd"))
                    yEnd = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Enemies"))
                    foreach (object enemy in element.Value as SimpleList<object>)
                        enemies.Add(new EnemyInstance(enemy as PairList<string, object>));
        }

        /// <summary>
        /// ToString override
        /// returns data in a list
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = "{xStart:" + xStart + ",yStart:" + yStart + ",xEnd:" + xEnd + ",yEnd:" + yEnd + ",Enemies:[";
            foreach (EnemyInstance e in enemies)
                s += e.ToString() + ",";
            if (enemies.Count > 0)
                s = s.Substring(0, s.Length - 1);
            return s + "]}";
        }
    }
}
