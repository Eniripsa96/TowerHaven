using System;
using System.Collections.Generic;
using System.IO;

namespace WPFEditor
{
    /// <summary>
    /// Map data class
    /// </summary>
    class MapData
    {
        /// <summary>
        /// Directory for map data files
        /// </summary>
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\MapsSource";
        
        /// <summary>
        /// Checks if the map exists
        /// </summary>
        /// <param name="mapName"></param>
        /// <returns></returns>
        public static Boolean MapExists(string mapName)
        {
            string[] names = GetMapNames();
            List<string> nameList = new List<string>();
            foreach (string s in nameList)
                if (s.Contains(".png"))
                    nameList.Add(s);
            names = nameList.ToArray();
            foreach (string s in names)
                if (s.Equals(mapName))
                    return true;
            return false;
        }

        /// <summary>
        /// Gets the list of created map names
        /// </summary>
        /// <returns></returns>
        public static string[] GetMapNames()
        {
            string[] names = Directory.GetFiles(directory);
            if (names.Length == 0)
                return new string[] { "New Map 1" };
            for (int i = 0; i < names.Length; ++i)
                names[i] = names[i].Substring(names[i].LastIndexOf("\\") + 1).Replace(".txt", "");
            return names;
        }
    }
}
