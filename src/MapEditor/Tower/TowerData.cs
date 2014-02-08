using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using JSON;
using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Tower data class
    /// </summary>
    class TowerData
    {
        /// <summary>
        /// Tower data files directory
        /// </summary>
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Towers";
        
        /// <summary>
        /// List of tower data
        /// </summary>
        private static List<Tower> towers;

        /// <summary>
        /// Returns the list of tower names
        /// </summary>
        /// <returns>tower name list</returns>
        public static string[] GetTowerNames()
        {
            string[] towerNames;
            towerNames = Directory.GetFiles(directory);
            List<string> towers = new List<string>();
            foreach (string s in towerNames)
                if (s.Contains(".png"))
                    towers.Add(s.Substring(s.LastIndexOf("\\") + 1).Replace(".png", ""));
            towerNames = towers.ToArray();
            return towerNames;
        }

        /// <summary>
        /// Gets the data of the tower with the given name
        /// </summary>
        /// <param name="towerName">tower name</param>
        /// <returns>JSON data string</returns>
        public static string GetTowerData(string towerName)
        {
            foreach (Tower t in towers)
                if (t.name.Equals(towerName))
                    return t.ToString();
            return "";
        }

        /// <summary>
        /// Loads tower data
        /// </summary>
        public static void Load()
        {
            towers = new List<Tower>();
            try
            {
                using (StreamReader read = new StreamReader(directory + "\\Data"))
                {
                    JSONParser parser = new JSONParser(read.ReadToEnd());
                    foreach (PairList<string, object> collection in parser.Objects)
                        foreach (PairListNode<string, object> element in collection)
                            if (element.Key.Equals("Towers"))
                                foreach (PairList<string, object> tower in element.Value as SimpleList<object>)
                                    towers.Add(new Tower(tower));
                }
            }
            catch (Exception) { }
        }
        
        /// <summary>
        /// Saves tower data
        /// </summary>
        public static void Save()
        {
            try
            {
                using (StreamWriter save = new StreamWriter(directory + "\\Data"))
                {
                    save.Write("{Towers:[");
                    string data = "";
                    foreach (Tower t in towers)
                        data += t.ToString() + ",";
                    if (data.Length > 0)
                        save.Write(data.Substring(0, data.Length - 1));
                    save.Write("]}");
                    save.Close();
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Gets the tower with the given name
        /// </summary>
        /// <param name="towerName">tower name</param>
        /// <returns>tower object</returns>
        public static Tower GetTower(string towerName)
        {
            foreach (Tower t in towers)
                if (t.name.Equals(towerName))
                    return t;
            return new Tower(towerName);
        }
        
        /// <summary>
        /// Updates the given tower
        /// </summary>
        /// <param name="tower">updated tower</param>
        public static void UpdateTower(Tower tower)
        {
            foreach (Tower t in towers)
                if (t.name.Equals(tower.name))
                {
                    t.range = tower.range;
                    t.sellValue = tower.sellValue;
                    t.status = tower.status;
                    t.delay = tower.delay;
                    t.damage = tower.damage;
                    t.buildCost = tower.buildCost;
                    return;
                }
            towers.Add(tower);
        }
    }
}
