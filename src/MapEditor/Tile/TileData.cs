using System;
using System.Collections.Generic;
using System.IO;

using JSON;
using TowerHaven;
using TowerHaven.Properties;

namespace WPFEditor
{
    /// <summary>
    /// Tile data class
    /// </summary>
    class TileData
    {
        /// <summary>
        /// Tile data files directory
        /// </summary>
        private static readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Tiles";

        /// <summary>
        /// Tile data splitter
        /// </summary>
        private static readonly char[] splitter = { ':' };

        /// <summary>
        /// Tile type hashes
        /// </summary>
        private static PairList<string, bool?> tileTypes;

        /// <summary>
        /// Gets the list of tile names
        /// </summary>
        /// <returns>tile name list</returns>
        public static string[] GetTileNames()
        {
            string[] spriteNames;
            try
            {
                spriteNames = Directory.GetFiles(directory);
            }
            catch (Exception)
            {
                LoadTiles();
                spriteNames = Directory.GetFiles(directory);
            }
            List<string> tileList = new List<string>();
            foreach (string s in spriteNames)
                if (s.Contains(".png"))
                    tileList.Add(s.Substring(s.LastIndexOf("\\") + 1).Replace(".png", ""));
            spriteNames = tileList.ToArray();
            return spriteNames;
        }

        /// <summary>
        /// Loads tile data
        /// </summary>
        public static void Load()
        {
            tileTypes = new PairList<string,bool?>();
            try
            {
                using (StreamReader read = new StreamReader(directory + "\\Data"))
                {
                    JSONParser parser = new JSONParser(read.ReadToEnd());
                    foreach (PairList<string, object> collection in parser.Objects)
                        foreach (PairListNode<string, object> element in collection)
                            if (element.Key.Equals("Tiles"))
                                foreach (string tile in element.Value as SimpleList<object>)
                                {
                                    string[] details = tile.Split(splitter);
                                    tileTypes.Add(details[0], details[1] == "null" ? null : (bool?) bool.Parse(details[1]));
                                }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Saves tile data
        /// </summary>
        public static void Save()
        {
            try
            {
                using (StreamWriter save = new StreamWriter(directory + "\\Data"))
                {
                    save.Write("{Tiles:[");
                    string data = "";
                    foreach (PairListNode<string, bool?> tile in tileTypes)
                        data += tile.Key + ":" + (tile.Value == null ? "null" : tile.Value.ToString()) + ",";
                    if (data.Length > 0)
                        save.Write(data.Substring(0, data.Length - 1));
                    save.Write("]}");
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Gets the type of the status with the given name
        /// </summary>
        /// <param name="tileName">tile name</param>
        /// <returns>tile type</returns>
        public static Boolean? GetType(string tileName)
        {
            foreach (PairListNode<string, bool?> pair in tileTypes)
                if (pair.Key.Equals(tileName))
                    return pair.Value;
            return true;
        }

        /// <summary>
        /// Sets the type of the given tile to the given value
        /// </summary>
        /// <param name="tileName">tile name</param>
        /// <param name="value">tile type</param>
        public static void SetType(string tileName, bool? value)
        {
            foreach (PairListNode<string, bool?> pair in tileTypes)
                if (pair.Key.Equals(tileName))
                {
                    pair.Value = value;
                    return;
                }
            tileTypes.Add(tileName, value);
        }

        /// <summary>
        /// Copies default sprites to their respective directories
        /// </summary>
        public static void LoadTiles()
        {
            Resources.Bricks.Save(directory + "\\Bricks.png");
            Resources.DeepWater.Save(directory + "\\DeepWater.png");
            Resources.Dirt.Save(directory + "\\Dirt.png");
            Resources.Grass.Save(directory + "\\Grass.png");
            Resources.Sand.Save(directory + "\\Sand.png");
            Resources.Rubble.Save(directory + "\\Rubble.png");
            Resources.ShallowWater.Save(directory + "\\ShallowWater.png");
            Resources.Marker.Save(directory.Replace("\\Tiles", "\\Marker.png"));
        }
    }
}
