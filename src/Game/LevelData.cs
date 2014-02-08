using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using WPFEditor;
using TowerHaven.AI;
using JSON;

namespace TowerHaven
{
    public class BasicLevel : Level
    {
        # region fields

        /// <summary>
        /// Wave data
        /// </summary>
        private List<Wave> waves;

        # endregion fields

        # region properties

        /// <summary>
        /// Returns the waves of the map
        /// </summary>
        public List<Wave> Waves
        {
            get { return waves; }
        }

        # endregion properties

        # region accessor methods

        /// <summary>
        /// Returns the number of waves remaining in the level
        /// </summary>
        /// <param name="index">current wave</param>
        /// <returns>number of waves left</returns>
        public override string GetRemainingWaves(int index)
        {
            return (waves.Count - index).ToString();
        }

        /// <summary>
        /// Gets the wave at the given index
        /// </summary>
        /// <param name="x">wave index</param>
        /// <returns></returns>
        public override Wave GetWave(int x)
        {
            return waves[x];
        }

        # endregion accessor methods

        # region IO

        /// <summary>
        /// Loads the map with the given path
        /// </summary>
        /// <param name="path">map path</param>
        /// <returns>true if successful and map is playable, false otherwise</returns>
        public override bool LoadMap(string path)
        {
            this.path = path;

            // Clear all data from previous maps
            try
            {
                string[] tempFiles = Directory.GetFiles(directory + "temp");
                foreach (string s in tempFiles)
                    File.Delete(s);
            }
            catch (Exception) { }

            // Set the map name
            mapName = path.Substring(path.LastIndexOf("\\") + 1).Replace(".thld", "");
            
            try
            {
                // Load sprites
                string[] categoryNames = { "Tile", "Tower", "Enemy" };
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    for (int collection = 0; collection < 3; ++collection)
                    {
                        // Read the collection size
                        byte[] collectionSizeBytes = new byte[2];
                        stream.Read(collectionSizeBytes, 0, 2);
                        short collectionSize = BitConverter.ToInt16(collectionSizeBytes, 0);

                        // Load each image in the collection
                        for (short i = 0; i < collectionSize; ++i)
                        {
                            // Read the image name
                            short nameLength = (short)stream.ReadByte();
                            string name = "";
                            for (int nameIndex = 0; nameIndex < nameLength; ++nameIndex)
                            {
                                byte[] character = new byte[2];
                                stream.Read(character, 0, 2);
                                name += BitConverter.ToChar(character, 0);
                            }
                            if (name.Contains("/"))
                                Directory.CreateDirectory(directory + "temp\\" + categoryNames[collection] + name.Substring(0, name.IndexOf("/")));

                            // Read the image size
                            byte[] imageSizeBytes = new byte[4];
                            stream.Read(imageSizeBytes, 0, 4);
                            int imageSize = BitConverter.ToInt32(imageSizeBytes, 0);

                            // Read the image
                            byte[] imageBytes = new byte[imageSize];
                            stream.Read(imageBytes, 0, imageSize);

                            // Save the image
                            using (FileStream copyStream = new FileStream(directory + "temp\\" + categoryNames[collection] + name + ".png", FileMode.Create))
                            {
                                copyStream.Write(imageBytes, 0, imageSize);
                            }
                        }
                    }
                }

                // Initialize lists
                builtTowers = new List<TowerInstance>();
                tiles = new PairList<string, bool?>();
                towers = new List<Tower>();
                enemies = new List<Enemy>();
                statuses = new List<Status>();
                waves = new List<Wave>();

                // Load level data
                using (StreamReader read = new StreamReader(path))
                {
                    // Skip over image data
                    string line;
                    do
                    {
                        line = read.ReadLine();
                    }
                    while (!line.Equals("[END OF SPRITE IMAGE DATA]"));
                    
                    // Read level JSON data
                    JSONParser parser = new JSONParser(read.ReadToEnd());
                    foreach (PairList<string, object> collection in parser.Objects)
                        foreach (PairListNode<string, object> data in collection)
                        {
                            if (data.Key.Equals("MapProperties"))
                            {
                                foreach (PairListNode<string, object> property in data.Value as PairList<string, object>)
                                    if (property.Key.Equals("Money"))
                                        money = int.Parse(property.Value.ToString());
                                    else if (property.Key.Equals("Health"))
                                        health = int.Parse(property.Value.ToString());
                                    else if (property.Key.Equals("Width"))
                                        width = int.Parse(property.Value.ToString());
                                    else if (property.Key.Equals("Height"))
                                        height = int.Parse(property.Value.ToString());
                                    else if (property.Key.Equals("Details"))
                                    {
                                        SimpleList<object> details = property.Value as SimpleList<object>;
                                        creator = details[0].ToString();
                                        date = details[1].ToString();
                                        description = details[2].ToString();
                                    }
                            }
                            else if (data.Key.Equals("MapTiles"))
                            {
                                int index = 0;
                                map = new string[width, height];
                                foreach (string tile in data.Value as SimpleList<object>)
                                {
                                    string[] details = tile.Split(splitter);
                                    for (int i = 0; i < int.Parse(details[1]); ++i)
                                        map[index % width, index++ / width] = details[0];
                                }
                            }
                            else if (data.Key.Equals("Waves"))
                                foreach (object wave in data.Value as SimpleList<object>)
                                    waves.Add(new Wave(wave as PairList<string, object>));
                            else if (data.Key.Equals("Tiles"))
                                foreach (string tile in data.Value as SimpleList<object>)
                                {
                                    string[] details = tile.Split(splitter);
                                    tiles.Add(details[0], details[1] == "null" ? null : (bool?) bool.Parse(details[1]));
                                }
                            else if (data.Key.Equals("Towers"))
                                foreach (object tower in data.Value as SimpleList<object>)
                                    towers.Add(new Tower(tower as PairList<string, object>));
                            else if (data.Key.Equals("Enemies"))
                                foreach (object enemy in data.Value as SimpleList<object>)
                                    enemies.Add(new Enemy(enemy as PairList<string, object>));
                            else if (data.Key.Equals("Statuses"))
                                foreach (object status in data.Value as SimpleList<object>)
                                    statuses.Add(new Status(status as PairList<string, object>));
                        }
                }
            }

            // Problems with the map
            catch (Exception) 
            {
                return BadMap("The map data is corrupt");
            }
            if (waves.Count == 0)
                return BadMap("The map has no waves");
            if (enemies.Count == 0)
                return BadMap("The map has no enemies");
            if (towers.Count == 0)
                return BadMap("The map has no towers");
            Boolean hasContent = false;
            for (int i = 0; i < waves.Count; ++i)
                for (int j = 0; j < waves[i].spawns.Count; ++j) 
                    if (waves[i].spawns[j].enemies.Count > 0)
                        hasContent = true;
                    else
                    {
                        waves[i].spawns.RemoveAt(j);
                        j--;
                        if (waves[i].spawns.Count == 0)
                        {
                            waves.RemoveAt(i);
                            i--;
                            j = 0;
                        }
                    }
            if (!hasContent)
                return BadMap("The map has no wave content");
            foreach (PairListNode<string, bool?> tile in tiles)
                if (!File.Exists(directory + "Temp\\Tile" + tile.Key + ".png"))
                    return BadMap("Tile sprites are missing from the map data");
            foreach (Enemy e in enemies)
                if (!File.Exists(directory + "Temp\\Enemy" + e.Name + ".png"))
                    return BadMap("Enemy sprites are missing from the map data");
            foreach (Tower t in towers)
                if (!File.Exists(directory + "Temp\\Tower" + t.name + ".png"))
                    return BadMap("Tower sprites are missing from the map data");
            foreach (Tower t in towers)
            {
                if (t.status.Equals("None"))
                    continue;
                Boolean statusLoaded = false;
                foreach (Status s in statuses)
                    if (s.name.Equals(t.status))
                        statusLoaded = true;
                if (!statusLoaded)
                    return BadMap("Statuses are missing from the map data");
            }

            return true;
        }

        # endregion IO
    }
}
