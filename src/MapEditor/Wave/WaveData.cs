using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Wave data class
    /// </summary>
    class WaveData
    {
        /// <summary>
        /// wave list
        /// </summary>
        private static List<Wave> waves;

        /// <summary>
        /// Wave JSON data
        /// </summary>
        public static string Data
        {
            get
            {
                string data = "Waves:[";
                foreach (Wave w in waves)
                    data += w + ",";
                if (waves.Count > 0)
                    data = data.Substring(0, data.Length - 1);
                return data + "]";
            }
        }

        /// <summary>
        /// Returns the list of enemies used in waves
        /// </summary>
        /// <returns>used enemy name list</returns>
        public static string[] GetEnemyList()
        {
            List<string> nameList = new List<string>();
            foreach (Wave w in waves)
                foreach (SpawnPoint s in w.spawns)
                    foreach (EnemyInstance e in s.enemies)
                        if (!nameList.Contains(e.name))
                            nameList.Add(e.name);
            return nameList.ToArray();
        }

        /// <summary>
        /// Returns the list of wave names
        /// </summary>
        /// <returns>wave names</returns>
        public static string[] GetWaveNames()
        {
            string[] waveNames = new string[waves.Count];
            for (int i = 0; i < waves.Count; ++i)
                waveNames[i] = "Wave" + (i + 1);
            return waveNames;
        }

        /// <summary>
        /// Initializes the wave list
        /// </summary>
        public static void Load()
        {
            waves = new List<Wave>();
        }

        /// <summary>
        /// Loads wave data
        /// </summary>
        /// <param name="data">wave data</param>
        public static void Load(PairList<string, object> data)
        {
            Wave w = new Wave(data);
            waves.Add(w);
        }

        /// <summary>
        /// Gets the wave object with the given id
        /// </summary>
        /// <param name="waveId">wave id</param>
        /// <returns>wave object</returns>
        public static Wave GetWave(int waveId)
        {
            if (waveId < waves.Count && waveId >= 0)
                return waves[waveId];
            Wave w = new Wave();
            waves.Add(w);
            return w;
        }

        /// <summary>
        /// Removes the wave with the given index
        /// </summary>
        /// <param name="index">wave index</param>
        public static void Remove(int index)
        {
            waves.RemoveAt(index);
        }

        /// <summary>
        /// Clears all waves
        /// </summary>
        public static void Clear()
        {
            waves.Clear();
        }
    }
}
