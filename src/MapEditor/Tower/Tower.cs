using System;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Tower Object
    /// </summary>
    public class Tower
    {
        /// <summary>
        /// Tower name
        /// </summary>
        public string name;

        /// <summary>
        /// Build cost of tower
        /// </summary>
        public int buildCost;
        
        /// <summary>
        /// Sell value of twer
        /// </summary>
        public int sellValue;

        /// <summary>
        /// Range of tower
        /// </summary>
        public int range;

        /// <summary>
        /// Damage of tower
        /// </summary>
        public int damage;

        /// <summary>
        /// Delay of tower
        /// </summary>
        public int delay;

        /// <summary>
        /// Radius of AOE attacks
        /// </summary>
        public int aoeRadius;

        /// <summary>
        /// Damage of AOE as a percentage (100 being full damage)
        /// </summary>
        public int aoeDamage;

        /// <summary>
        /// Whether or not the AOE applies the tower's status
        /// </summary>
        public Boolean aoeStatus;

        /// <summary>
        /// Status inflicted by tower
        /// </summary>
        public string status;

        public int moneyPerTick;
        public int moneyPerAttack;
        public int moneyPerKill;
        public int moneyPerWave;
        public string aura;

        /// <summary>
        /// Tower this upgrades from
        /// </summary>
        public string baseTower;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="buildCost">build cost</param>
        /// <param name="sellValue">sell value</param>
        /// <param name="range">range</param>
        /// <param name="damage">damage</param>
        /// <param name="delay">delay</param>
        /// <param name="status">status</param>
        public Tower(string name)
        {
            this.name = name;
            buildCost = 0;
            sellValue = 0;
            damage = 0;
            delay = 0;
            range = 1;
            status = "None";
            baseTower = "<None>";
            aoeRadius = 0;
            aoeDamage = 100;
            aoeStatus = false;
            moneyPerAttack = 0;
            moneyPerKill = 0;
            moneyPerTick = 0;
            moneyPerWave = 0;
            aura = "None";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">JSON data list</param>
        public Tower(PairList<string, object> data)
        {
            name = data[0].Key;
            data = data[0].Value as PairList<string, object>;
            foreach (PairListNode<string, object> element in data)
                if (element.Key.Equals("BuildCost"))
                    buildCost = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("SellValue"))
                    sellValue = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Range"))
                    range = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Damage"))
                    damage = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Delay"))
                    delay = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Status"))
                    status = element.Value.ToString();
                else if (element.Key.Equals("Upgrade"))
                    baseTower = element.Value.ToString();
                else if (element.Key.Equals("AOERadius"))
                    aoeRadius = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("AOEDamage"))
                    aoeDamage = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("AOEStatus"))
                    aoeStatus = bool.Parse(element.Value.ToString());
                else if (element.Key.Equals("MoneyPerTick"))
                    moneyPerTick = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("MoneyPerAttack"))
                    moneyPerAttack = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("MoneyPerKill"))
                    moneyPerKill = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("MoneyPerWave"))
                    moneyPerWave = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Aura"))
                    aura = element.Value.ToString();
        }
        
        /// <summary>
        /// ToString override
        /// Returns tower data in JSON format
        /// </summary>
        /// <returns>tower data</returns>
        public override string ToString()
        {
            return "{" + name + ":{BuildCost:" + buildCost + 
                ",SellValue:" + sellValue + 
                ",Range:" + range + 
                ",Damage:" + damage + 
                ",Delay:" + delay + 
                ",Status:" + status + 
                ",Upgrade:" + baseTower + 
                ",AOERadius:" + aoeRadius + 
                ",AOEDamage:" + aoeDamage + 
                ",AOEStatus:" + aoeStatus + 
                ",MoneyPerTick:" + moneyPerTick + 
                ",MoneyPerAttack:" + moneyPerAttack + 
                ",MoneyPerKill:" + moneyPerKill + 
                ",MoneyPerWave:" + moneyPerWave + 
                ",Aura:" + aura + "}}";
        }
    }
}
