using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using WPFEditor;
using TowerHaven.AI;
using JSON;

namespace TowerHaven
{
    public abstract class Level : _2DMap
    {
        # region constants

        /// <summary>
        /// Level data directory
        /// </summary>
        protected readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\";

        /// <summary>
        /// Splitter character for JSON elements
        /// </summary>
        protected readonly char[] splitter = new char[] { ':' };

        # endregion constants

        # region fields

        /// <summary>
        /// Tile data
        /// </summary>
        protected PairList<string, bool?> tiles;

        /// <summary>
        /// Status data
        /// </summary>
        protected List<Status> statuses;

        /// <summary>
        /// Enemy data
        /// </summary>
        protected List<Enemy> enemies;

        /// <summary>
        /// Tower data
        /// </summary>
        protected List<Tower> towers;

        /// <summary>
        /// Active towers
        /// </summary>
        protected List<TowerInstance> builtTowers;

        /// <summary>
        /// Map Data
        /// </summary>
        protected string[,] map;

        /// <summary>
        /// Map description
        /// </summary>
        protected string description;

        /// <summary>
        /// Map name
        /// </summary>
        protected string mapName;

        /// <summary>
        /// Map path
        /// </summary>
        protected string path;

        /// <summary>
        /// Map creator
        /// </summary>
        protected string creator;

        /// <summary>
        /// Map creation date
        /// </summary>
        protected string date;

        /// <summary>
        /// Current health
        /// </summary>
        protected int health;

        /// <summary>
        /// Current money
        /// </summary>
        protected int money;

        /// <summary>
        /// Map width
        /// </summary>
        protected int width;

        /// <summary>
        /// Map height
        /// </summary>
        protected int height;

        # endregion fields

        # region properties

        /// <summary>
        /// Checks if at least one tower can be built
        /// </summary>
        public Boolean CanBuild
        {
            get
            {
                foreach (Tower t in towers)
                    if (money >= t.buildCost && t.baseTower.Equals("<None>"))
                        return true;
                return false;
            }
        }

        /// <summary>
        /// Built tower property
        /// </summary>
        public List<TowerInstance> Towers
        {
            get { return builtTowers; }
        }

        /// <summary>
        /// Tower list property
        /// </summary>
        public List<Tower> TowerList
        {
            get { return towers; }
        }

        /// <summary>
        /// Map property
        /// </summary>
        public string[,] Map
        {
            get { return map; }
        }

        /// <summary>
        /// Description property
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Name property
        /// </summary>
        public string MapName
        {
            get { return mapName; }
        }

        /// <summary>
        /// Creator property
        /// </summary>
        public string Creator
        {
            get { return creator; }
        }

        /// <summary>
        /// Date property
        /// </summary>
        public string Date
        {
            get { return date; }
        }

        /// <summary>
        /// Map Horizontal Size property
        /// </summary>
        public int HorizontalSize
        {
            get { return width; }
        }

        /// <summary>
        /// Map Vertical Size property
        /// </summary>
        public int VerticalSize
        {
            get { return height; }
        }

        /// <summary>
        /// Health property
        /// </summary>
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        /// <summary>
        /// Money property
        /// </summary>
        public int Money
        {
            get { return money; }
            set { money = value; }
        }

        # endregion properties

        # region accessor methods

        /// <summary>
        /// Gets the list of upgraded towers that t can turn into
        /// </summary>
        /// <param name="t">tower</param>
        /// <returns>upgrade towers</returns>
        public List<Tower> GetUpgrades(Tower t)
        {
            List<Tower> upgrades = new List<Tower>();
            foreach (Tower tower in towers)
                if (tower.baseTower.Equals(t.name) && tower.buildCost <= money)
                    upgrades.Add(tower);
            return upgrades;
        }

        /// <summary>
        /// Returns a list of towers able to be build right now
        /// </summary>
        /// <returns>tower list</returns>
        public List<Tower> GetBuildableTowers()
        {
            List<Tower> buildableTowers = new List<Tower>();
            foreach (Tower t in towers)
                if (money >= t.buildCost && t.baseTower.Equals("<None>"))
                    buildableTowers.Add(t);
            return buildableTowers;
        }

        /// <summary>
        /// Returns the number of waves remaining in the level
        /// </summary>
        /// <param name="index">current wave</param>
        /// <returns>number of waves left</returns>
        public abstract string GetRemainingWaves(int index);

        /// <summary>
        /// Returns the type of the tile at the given coordinates
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        /// <returns>type label</returns>
        public string GetTileType(int x, int y)
        {
            if (IsTower(x, y))
                return "Not Walkable";
            bool? type = tiles[map[x, y]];
            if (type == null)
                return "Not Walkable";
            else if (type == true)
                return "Buildable";
            else
                return "Walkable";
        }

        /// <summary>
        /// Returns whether or not the tile at the given coordinates can be built on
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        /// <returns>true if buildable, false otherwise</returns>
        public Boolean IsBuildable(int x, int y)
        {
            if (tiles[map[x, y]] == true)
                return true;
            return false;
        }

        /// <summary>
        /// Returns if the given coordinates is occupied by a tower
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        /// <returns></returns>
        public Boolean IsTower(int x, int y)
        {
            foreach (TowerInstance t in builtTowers)
                if (t.HorizontalPosition == x && t.VerticalPosition == y)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns the tile at the given coordinate in the map
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        /// <returns></returns>
        public string GetTileName(int x, int y)
        {
            foreach (TowerInstance t in builtTowers)
                if (t.HorizontalPosition == x && t.VerticalPosition == y)
                    return t.Name;
            return map[x, y];
        }

        /// <summary>
        /// Returns the tower name at the given index in the list
        /// </summary>
        /// <param name="x">index</param>
        /// <returns></returns>
        public string GetTowerName(int x)
        {
            return towers[x % towers.Count].name;
        }

        /// <summary>
        /// Gets the wave at the given index
        /// </summary>
        /// <param name="x">wave index</param>
        /// <returns></returns>
        public abstract Wave GetWave(int x);

        /// <summary>
        /// Returns if the tile at the given coordinates is walkable
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        /// <returns></returns>
        public Boolean IsWalkable(int x, int y)
        {
            return tiles[map[x, y]] != null && !IsTower(x, y);
        }

        /// <summary>
        /// Gets the tower with the given name
        /// </summary>
        /// <param name="name">tower name</param>
        /// <returns>tower object</returns>
        public Tower GetTower(string name)
        {
            foreach (Tower t in towers)
                if (t.name.Equals(name))
                    return t;
            return null;
        }

        /// <summary>
        /// Gets the enemy with the given name
        /// </summary>
        /// <param name="name">enemy name</param>
        /// <returns>enemy object</returns>
        public Enemy GetEnemy(string name)
        {
            foreach (Enemy e in enemies)
                if (e.Name.Equals(name))
                    return e;
            return null;
        }

        /// <summary>
        /// Gets the status with the given name
        /// </summary>
        /// <param name="name">status name</param>
        /// <returns></returns>
        public Status GetStatus(string name)
        {
            foreach (Status s in statuses)
                if (s.name.Equals(name))
                    return s;
            return null;
        }

        # endregion accessor methods

        # region actions

        /// <summary>
        /// Upgrades the tower at the given coordinates to the tower with the given name
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        /// <param name="upgrade">upgraded tower name</param>
        public void UpgradeTower(int x, int y, string upgrade)
        {
            foreach (TowerInstance t in builtTowers)
                if (t.HorizontalPosition == x && t.VerticalPosition == y)
                {
                    builtTowers.Remove(t);
                    builtTowers.Add(new TowerInstance(upgrade, x, y));
                    money -= GetTower(upgrade).buildCost;
                    return;
                }
        }

        /// <summary>
        /// Buys the supplied tower and marks it at the given coordinates
        /// </summary>
        /// <param name="t">tower to buy</param>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        public void BuyTower(Tower t, int x, int y)
        {
            builtTowers.Add(new TowerInstance(t.name, x, y));
            money -= t.buildCost;
        }

        /// <summary>
        /// Sells the tower at the given coordinates
        /// </summary>
        /// <param name="x">horizontal coordinate</param>
        /// <param name="y">vertical coordinate</param>
        public void SellTower(int x, int y)
        {
            foreach (TowerInstance t in builtTowers)
                if (t.HorizontalPosition == x && t.VerticalPosition == y)
                {
                    builtTowers.Remove(t);
                    money += GetTower(t.Name).sellValue;
                    return;
                }
        }

        # endregion actions

        # region IO

        /// <summary>
        /// Loads the map with the given name
        /// </summary>
        /// <param name="mName">map name</param>
        /// <returns>true if load was successful and map is playable, false otherwise</returns>
        public Boolean Load(string mName)
        {
            return Load(mName, "Maps\\");
        }

        /// <summary>
        /// Loads the campaign map with the given name
        /// </summary>
        /// <param name="mName">map name</param>
        /// <returns>true if load was successful and map is playable, false otherwise</returns>
        public Boolean LoadCampaign(string mName)
        {
            return Load(mName, "Campaign\\");
        }

        /// <summary>
        /// Loads the map with the given name and folder
        /// </summary>
        /// <param name="mName">map name</param>
        /// <param name="folder">folder name</param>
        /// <returns>true if successful and map is playable, false otherwise</returns>
        protected Boolean Load(string mName, string folder)
        {
            return LoadMap(directory + folder + mName + ".thld");
        }

        /// <summary>
        /// Loads the map with the given path
        /// </summary>
        /// <param name="path">map path</param>
        /// <returns>true if successful and map is playable, false otherwise</returns>
        public abstract bool LoadMap(string path);
        
        # endregion IO

        # region display

        /// <summary>
        /// Gives a message describing a problem with the map and deletes the map
        /// </summary>
        /// <param name="message">error message</param>
        /// <returns>false</returns>
        protected Boolean BadMap(string message)
        {
            MessageBox.Show(message);
            try
            {
                File.Delete(path);
            }
            catch (Exception) { }
            return false;
        }

        # endregion display
    }
}
