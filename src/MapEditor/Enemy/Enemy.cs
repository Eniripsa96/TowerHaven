using System;
using System.Collections.Generic;
using System.Windows.Controls;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Enemy Object
    /// </summary>
    public class Enemy
    {
        /// <summary>
        /// Enemy is damaged
        /// </summary>
        public event EventHandler Damaged;

        /// <summary>
        /// Name for the enemy
        /// </summary>
        private string name;

        /// <summary>
        /// Name of the enemy
        /// </summary>
        private int health;
        
        /// <summary>
        /// Speed of the enemy
        /// </summary>
        public int speed;

        /// <summary>
        /// Mode index of the enemy
        /// </summary>
        public int move;

        /// <summary>
        /// Damage dealt when leaked
        /// </summary>
        public int damage;

        /// <summary>
        /// Reward for killing the enemy
        /// </summary>
        private int reward;

        /// <summary>
        /// Spawn point originated from
        /// </summary>
        public int spawn;

        /// <summary>
        /// Statuses the enemy is immune to
        /// </summary>
        public string immune;

        /// <summary>
        /// Whether or not the unit is a flying unit
        /// </summary>
        public Boolean flying;

        /// <summary>
        /// The enemy sprite
        /// </summary>
        public Canvas sprite;

        /// <summary>
        /// Frames since last move
        /// </summary>
        private int ticks;

        /// <summary>
        /// Inflicted statuses
        /// </summary>
        private List<Status> statuses;

        /// <summary>
        /// Name property
        /// </summary>
        public string Name
        {
            get { return name; }
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
        /// Reward Property
        /// </summary>
        public int Reward
        {
            get { return reward; }
            set { reward = value; }
        }

        /// <summary>
        /// Speed property
        /// </summary>
        public int Speed
        {
            get
            {
                int multiplier = 1;
                int bonus = 0;
                foreach (Status s in statuses)
                {
                    if (s.slowDuration > 0)
                    {
                        multiplier *= s.slowMultiplier;
                        bonus += s.slowBonus;
                    }
                }
                return speed * multiplier + bonus;
            }
        }

        public int Fear
        {
            get
            {
                foreach (Status s in statuses)
                    if (s.fearDuration > 0)
                        return -1;
                return 1;
            }
        }

        /// <summary>
        /// Property for if the enemy can move or not
        /// </summary>
        public Boolean CanMove
        {
            get
            {
                int damage = 0;
                Boolean stunned = false;
                foreach (Status s in statuses)
                {
                    if (s.dotDuration > 0)
                        damage += s.dotMoveDamage;
                    if (s.stunDuration > 0)
                        stunned = true;
                }
                if (ticks <= Speed)
                {
                    if (!stunned)
                        ticks++;
                }
                else
                {
                    health -= damage;
                    Damaged.Invoke(this, null);
                }
                return ticks > Speed;
            }
        }

        /// <summary>
        /// Sets the enemy's ticks
        /// </summary>
        public int Ticks
        {
            set { ticks = value; }
        }

        /// <summary>
        /// Immunities of the enemy as a list
        /// </summary>
        public string[] ImmunitiesList
        {
            get
            {
                return immune.Split(';');
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">name</param>
        public Enemy(string name)
            : this(name, 1, 0, 1, 0, "None", false)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="health">health</param>
        /// <param name="speed">speed</param>
        /// <param name="immune">immunities</param>
        public Enemy(string name, int health, int speed, int damage, int reward, string immune, Boolean flying)
        {
            this.name = name;
            this.health = health;
            this.speed = speed;
            this.damage = damage;
            this.reward = reward;
            this.immune = immune;
            this.flying = flying;
            statuses = new List<Status>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">JSON data list</param>
        public Enemy(PairList<string, object> data)
        {
            name = data[0].Key;
            data = data[0].Value as PairList<string, object>;
            foreach (PairListNode<string, object> element in data)
                if (element.Key.Equals("Health"))
                    health = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Speed"))
                    speed = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Damage"))
                    damage = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Reward"))
                    reward = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Immune"))
                    immune = element.Value.ToString();
                else if (element.Key.Equals("Flying"))
                    flying = bool.Parse(element.Value.ToString());
        }

        /// <summary>
        /// Modifies the status applied to the enemy each frame
        /// </summary>
        public void Tick()
        {
            try
            {
                foreach (Status s in statuses)
                {

                    if (s.dotDuration > 0)
                        health -= s.dotFrameDamage;
                    if (s.dotDuration > 0)
                        s.dotDuration--;
                    if (s.extraDamageDuration > 0)
                        s.extraDamageDuration--;
                    if (s.stunDuration > 0)
                        s.stunDuration--;
                    if (s.slowDuration > 0)
                        s.slowDuration--;
                    if (s.fearDuration > 0)
                        s.fearDuration--;
                    if (s.dotDuration + s.extraDamageDuration + s.stunDuration + s.slowDuration + s.fearDuration == 0)
                        statuses.Remove(s);
                }
            }
            catch (Exception) { Tick(); }
        }

        /// <summary>
        /// Inflicts the enemy with the status
        /// </summary>
        /// <param name="s">name of status</param>
        public void Inflict(Status s)
        {
            if (immune.Contains(s.name))
                return;
            foreach (Status status in statuses)
                if (status.name.Equals(s.name))
                {
                    status.stunDuration = s.stunDuration;
                    status.slowDuration = s.slowDuration;
                    status.extraDamageDuration = s.extraDamageDuration;
                    status.dotDuration = s.dotDuration;
                    if (s.fearDuration > 0)
                        status.fearDuration = s.fearDuration;
                    return;
                }
            statuses.Add(s);
        }

        /// <summary>
        /// Deals damage to the enemy
        /// </summary>
        /// <param name="damage">damage amount</param>
        public void Damage(int damage)
        {
            int multiplier = 1;
            int bonus = 0;
            foreach (Status s in statuses)
                if (s.extraDamageDuration > 0)
                {
                    multiplier *= s.extraDamageMultiplier;
                    bonus += s.extraDamageBonus;
                }
            health -= damage * multiplier + bonus;
            if (damage * multiplier + bonus > 0)
                Damaged.Invoke(this, null);
        }

        /// <summary>
        /// Clones the enemy
        /// </summary>
        /// <returns></returns>
        public Enemy Clone()
        {
            return new Enemy(name, health, speed, damage, reward, immune, flying);
        }

        /// <summary>
        /// CompareTo Implementation
        /// </summary>
        /// <param name="obj">object being compared to</param>
        /// <returns>1 if closer to finish, 0 if the same, -1 otherwise</returns>
        public int CompareTo(object obj)
        {
            if (!(obj is Enemy))
                throw new ArgumentException("Object is not an enemy!");
            Enemy e = obj as Enemy;
            if (flying && !e.flying)
                return 1;
            if (!flying && e.flying)
                return -1;
            if (e.move > move)
                return -1;
            else if (e.move == move)
                return 0;
            else
                return 1;
        }

        /// <summary>
        /// ToString override
        /// provides enemy data in JSON format
        /// </summary>
        /// <returns>enemy data</returns>
        public override string ToString()
        {
            return "{" + name + ":{Health:" + health + ",Speed:" + speed + ",Damage:" + damage + ",Reward:" + reward + ",Immune:" + immune + ",Flying:" + flying + "}}";
        }
    }
}