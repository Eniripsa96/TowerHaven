using System;
using System.Collections.Generic;
using System.IO;

using JSON;
using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Enemy data class
    /// </summary>
    class EnemyData
    {
        /// <summary>
        /// Enemy data files directory
        /// </summary>
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Enemies";
        
        /// <summary>
        /// List of enemy data
        /// </summary>
        private static List<Enemy> enemies;

        /// <summary>
        /// Returns a list of all enemies
        /// </summary>
        /// <returns>enemy name list</returns>
        public static string[] GetEnemyNames()
        {
            string[] spriteNames;
            spriteNames = Directory.GetFiles(directory);
            List<string> tileList = new List<string>();
            foreach (string s in spriteNames)
                if (s.Contains(".png"))
                    tileList.Add(s.Substring(s.LastIndexOf("\\") + 1).Replace(".png", ""));
            spriteNames = tileList.ToArray();
            return spriteNames;
        }

        /// <summary>
        /// Gets the JSON data of the enemy
        /// </summary>
        /// <param name="enemyName">name of the enemy</param>
        /// <returns>JSON data string</returns>
        public static string GetEnemyData(string enemyName)
        {
            foreach (Enemy e in enemies)
                if (e.Name.Equals(enemyName))
                    return e.ToString();
            return "";
        }

        /// <summary>
        /// Loads enemy data
        /// </summary>
        public static void Load()
        {
            enemies = new List<Enemy>();
            try
            {
                using (StreamReader read = new StreamReader(directory + "\\Data"))
                {
                    JSONParser parser = new JSONParser(read.ReadToEnd());
                    foreach (PairList<string, object> collection in parser.Objects)
                        foreach (PairListNode<string, object> element in collection)
                            if (element.Key.Equals("Enemies"))
                                foreach (PairList<string, object> enemy in element.Value as SimpleList<object>)
                                    enemies.Add(new Enemy(enemy));
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Saves enemy data
        /// </summary>
        public static void Save()
        {
            try
            {
                using (StreamWriter save = new StreamWriter(directory + "\\Data"))
                {
                    save.Write("{Enemies:[");
                    string data = "";
                    foreach (Enemy e in enemies)
                        data += e.ToString() + ",";
                    if (data.Length > 0)
                        save.Write(data.Substring(0, data.Length - 1));
                    save.Write("]}");
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Gets the enemy with the given name
        /// </summary>
        /// <param name="enemyName">enemy name</param>
        /// <returns>enemy object</returns>
        public static Enemy GetEnemy(string enemyName)
        {
            foreach (Enemy e in enemies)
                if (e.Name.Equals(enemyName))
                    return e;
            return new Enemy(enemyName);
        }

        /// <summary>
        /// Updates the enemy or adds the enemy to the list if a new one
        /// </summary>
        /// <param name="enemy">updated enemy object</param>
        public static void UpdateEnemy(Enemy enemy)
        {
            foreach (Enemy e in enemies)
                if (e.Name.Equals(enemy.Name))
                {
                    e.Health = enemy.Health;
                    e.speed = enemy.speed;
                    e.immune = enemy.immune;
                    return;
                }
            enemies.Add(enemy);
        }
    }
}
