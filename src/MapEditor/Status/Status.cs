using System;

using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Status object
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Status name
        /// </summary>
        public string name;

        /// <summary>
        /// Duration of a stun
        /// </summary>
        public int stunDuration;

        /// <summary>
        /// Duration of a slow
        /// </summary>
        public int slowDuration;

        /// <summary>
        /// Slow multiplier (speed * multiplier)
        /// </summary>
        public int slowMultiplier;

        /// <summary>
        /// Slow bonus (speed + bonus)
        /// </summary>
        public int slowBonus;

        /// <summary>
        /// Duration for extra damage
        /// </summary>
        public int extraDamageDuration;

        /// <summary>
        /// Extra damage multiplier (damage * multiplier)
        /// </summary>
        public int extraDamageMultiplier;

        /// <summary>
        /// Extra damage bonus (damage + bonus)
        /// </summary>
        public int extraDamageBonus;

        /// <summary>
        /// Duration for DOT damage
        /// </summary>
        public int dotDuration;

        /// <summary>
        /// DOT damage per frame
        /// </summary>
        public int dotFrameDamage;

        /// <summary>
        /// DOT damage per move
        /// </summary>
        public int dotMoveDamage;

        /// <summary>
        /// Chance to fear
        /// </summary>
        public int fearChance;

        /// <summary>
        /// Duration of fear
        /// </summary>
        public int fearDuration;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name"></param>
        public Status(string name)
            : this(name, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="stunDuration">stun duration</param>
        /// <param name="slowDuration">slow duration</param>
        /// <param name="slowMultiplier">slow multiplier</param>
        /// <param name="slowBonus">slow bonus</param>
        /// <param name="extraDamageDuration">extra damage duration</param>
        /// <param name="extraDamageMultiplier">extra damage multiplier</param>
        /// <param name="extraDamageBonus">extra damage bonus</param>
        /// <param name="dotDuration">DOT duration</param>
        /// <param name="dotFrameDamage">DOT frame damage</param>
        /// <param name="dotMoveDamage">DOT move damage</param>
        public Status(string name, int stunDuration, int slowDuration, int slowMultiplier, int slowBonus, int extraDamageDuration, int extraDamageMultiplier, int extraDamageBonus, int dotDuration, int dotFrameDamage, int dotMoveDamage, int fearChance, int fearDuration)
        {
            this.name = name;
            this.stunDuration = stunDuration;
            this.slowBonus = slowBonus;
            this.slowMultiplier = slowMultiplier;
            this.slowDuration = slowDuration;
            this.extraDamageDuration = extraDamageDuration;
            this.extraDamageMultiplier = extraDamageMultiplier;
            this.extraDamageBonus = extraDamageBonus;
            this.dotDuration = dotDuration;
            this.dotFrameDamage = dotFrameDamage;
            this.dotMoveDamage = dotMoveDamage;
            this.fearChance = fearChance;
            this.fearDuration = fearDuration;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">JSON data list</param>
        public Status(PairList<string, object> data)
        {
            name = data[0].Key;
            data = data[0].Value as PairList<string, object>;
            foreach (PairListNode<string, object> element in data)
                if (element.Key.Equals("Stun"))
                    stunDuration = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("SlowD"))
                    slowDuration = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("SlowM"))
                    slowMultiplier = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("SlowB"))
                    slowBonus = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("ExtraD"))
                    extraDamageDuration = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("ExtraM"))
                    extraDamageMultiplier = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("ExtraB"))
                    extraDamageBonus = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("DOTD"))
                    dotDuration = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("DOTF"))
                    dotFrameDamage = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("DOTM"))
                    dotMoveDamage = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("FearC"))
                    fearChance = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("FearD"))
                    fearDuration = int.Parse(element.Value.ToString());
        }

        /// <summary>
        /// Clones the status
        /// </summary>
        /// <returns>clone</returns>
        public Status Clone()
        {
            return new Status(name, stunDuration, slowDuration, slowMultiplier, slowBonus, extraDamageDuration, extraDamageMultiplier, extraDamageBonus, dotDuration, dotFrameDamage, dotMoveDamage, fearChance, fearChance > new Random().Next(100) ? fearDuration : 0);
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>returns the status data in JSON format</returns>
        public override string ToString()
        {
            return "{" + name + ":{Stun:" + stunDuration + ",SlowD:" + slowDuration + ",SlowM:" + slowMultiplier + ",SlowB:" + slowBonus + ",ExtraD:" + extraDamageDuration + ",ExtraM:" + extraDamageMultiplier + ",ExtraB:" + extraDamageBonus + ",DOTD:" + dotDuration + ",DOTF:" + dotFrameDamage + ",DOTM:" + dotMoveDamage + ",FearC:" + fearChance + ",FearD:" + fearDuration + "}}";
        }
    }
}
