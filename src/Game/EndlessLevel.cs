using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using JSON;
using WPFEditor;
using TowerHaven.AI;

namespace TowerHaven
{
    /// <summary>
    /// Level data for endless mode
    /// </summary>
    class EndlessLevel : Level
    {
        # region constants

        /// <summary>
        /// The start location for spawns
        /// </summary>
        private readonly int[] start = { 0, 0 };

        /// <summary>
        /// The end location for spawns
        /// </summary>
        private readonly int[] end = { 29, 29 };

        /// <summary>
        /// The number of normal enemies
        /// </summary>
        private readonly int numberOfEnemies = 9;

        /// <summary>
        /// The number of boss enemies
        /// </summary>
        private readonly int numberOfBosses = 3;

        # endregion constants

        # region fields

        private Random random;
        private Wave currentWave;
        private int currentWaveId;

        # endregion fields

        # region accessor methods

        /// <summary>
        /// Returns the number of waves remaining in the level
        /// </summary>
        /// <param name="index">current wave</param>
        /// <returns>number of waves left</returns>
        public override string GetRemainingWaves(int index)
        {
            return "Infinite";
        }

        
        /// <summary>
        /// Gets the wave at the given index
        /// </summary>
        /// <param name="x">wave index</param>
        /// <returns></returns>
        public override Wave GetWave(int x)
        {
            // Create a new wave if trying to access the next wave
            if (x != currentWaveId)
            {
                // Initialize the wave
                currentWave = new Wave();
                currentWave.reward = 0;

                // Initialize the spawn point
                SpawnPoint spawn = new SpawnPoint();
                spawn.xStart = start[0];
                spawn.xEnd = end[0];
                spawn.yStart = start[1];
                spawn.yEnd = end[1];

                // Add a boss on every 10th wave
                if (x % 10 == 9)
                {
                    // Choose a random boss
                    int enemyId = random.Next(numberOfEnemies, numberOfEnemies + numberOfBosses);
                    Enemy enemy = enemies[enemyId];

                    // Get a random multiplier for the enemy health beween 0.9 and 1.1
                    double randomChange = random.Next(90, 111) / 100.0;

                    // Calculate the boss health depending on its speed, whether or not if flies, the wave, and the random multiplier
                    enemy.Health = 
                        (int)(Math.Pow(2, x / 10)  // Exponential Wave multiplier
                        * (enemy.speed + 4)        // Speed multiplier
                        * randomChange             // Random multiplier
                        * (enemy.flying ? 1 : 1.2) // Flying multiplier
                        * 100);                    // Boss Constant

                    // Calculate the boss reward depending on the wave (increases by 10 each boss spawn)
                    enemy.Reward = 10 * ((x + 1) / 10); 

                    // Create an enemy instance pointing to this boss
                    EnemyInstance instance = new EnemyInstance(enemy.Name, 0, 1);

                    // Add the enemy instance to the spawn point
                    spawn.enemies.Add(instance);
                }

                // Add 1 enemy spawn plus an additional for each 10 waves that have passed
                for (int i = 0; i < x / 10 + 1; ++i)
                {
                    // Choose a random enemy
                    int enemyId = random.Next(0, numberOfEnemies);
                    Enemy enemy = enemies[enemyId];

                    // Get a random multiplier for the enemy health between 0.9 and 1.1
                    double randomChange = random.Next(90, 111) / 100.0;

                    // Calculate the quantity depending on what wave it is (between 15 and 25, adding 10 to each every 25 waves)
                    int quantity = random.Next(15 + 10 * (x / 25), 25 + 15 * (x / 25));

                    // Calculate the enemy health depending on the enemy speed, whether or not it flies, the wave, and the random multiplier
                    enemy.Health = 
                        (int)((x + 5 * (x / 10 + 1)) // Wave multiplier 
                        * (enemy.speed + 4)          // Speed multiplier
                        * randomChange               // Random multiplier
                        * (enemy.flying ? 0.9 : 1.2) // Flying multiplier
                        * Math.Pow(2, x/25));        // Exponential multiplier

                    // Set the reward to 1, increasing every 50 waves
                    enemy.Reward = 1 + (x / 50);                  
      
                    // Create an enemy instance point to the created enemy
                    EnemyInstance instance = new EnemyInstance(enemy.Name, 15 / (x / 6 + 1), quantity);

                    // Add the enemy instance to the spawn point
                    spawn.enemies.Add(instance);
                }

                // Add the spawn point to the wave
                currentWave.spawns.Add(spawn);
            }

            // Return the current wave
            return currentWave;
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
            currentWaveId = -1;
            random = new Random();

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
                            else if (data.Key.Equals("Tiles"))
                                foreach (string tile in data.Value as SimpleList<object>)
                                {
                                    string[] details = tile.Split(splitter);
                                    tiles.Add(details[0], details[1] == "null" ? null : (bool?)bool.Parse(details[1]));
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
            if (enemies.Count == 0)
                return BadMap("The map has no enemies");
            if (towers.Count == 0)
                return BadMap("The map has no towers");
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

            GetWave(0);


            return true;
        }

        # endregion IO
    }
}
