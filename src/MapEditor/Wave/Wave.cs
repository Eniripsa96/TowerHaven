using System;
using System.Collections.Generic;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Wave object
    /// </summary>
    public class Wave
    {
        /// <summary>
        /// Monetary reward of the wave
        /// </summary>
        public int reward;

        /// <summary>
        /// Spawn points of the map
        /// </summary>
        public List<SpawnPoint> spawns;

        /// <summary>
        /// Empty Constructor (for blank waves)
        /// </summary>
        public Wave()
        {
            reward = 0;
            spawns = new List<SpawnPoint>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reward">reward</param>
        /// <param name="spawns">spawn points</param>
        public Wave(PairList<string, object> data)
        {
            spawns = new List<SpawnPoint>();
            foreach (PairListNode<string, object> element in data)
                if (element.Key.Equals("Reward"))
                    reward = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Spawns"))
                    foreach (object spawn in element.Value as SimpleList<object>)
                        spawns.Add(new SpawnPoint(spawn as PairList<string, object>));
        }

        /// <summary>
        /// ToString override
        /// Returns the wave data in JSON format
        /// </summary>
        /// <returns>wave data</returns>
        public override string ToString()
        {
            string s = "{Reward:" + reward + ",Spawns:[";
            foreach (SpawnPoint spawn in spawns)
            {
                s += spawn.ToString() + ",";
            }
            if (spawns.Count > 0)
                s = s.Substring(0, s.Length - 1);
            return (s + "]}");
        }
    }
}
