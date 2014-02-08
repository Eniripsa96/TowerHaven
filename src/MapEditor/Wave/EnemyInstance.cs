using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// EnemyInstance Object
    /// Enemy data for placing in maps
    /// </summary>
    public class EnemyInstance
    {
        /// <summary>
        /// Delay until spawning
        /// </summary>
        public int delay;

        /// <summary>
        /// Enemy Name
        /// </summary>
        public string name;

        /// <summary>
        /// Number of spawns of the instance
        /// </summary>
        public int quantity;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="delay">spawn delay</param>
        public EnemyInstance(string name, int delay, int quantity)
        {
            this.name = name;
            this.delay = delay;
            this.quantity = quantity;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">enemy data</param>
        public EnemyInstance(PairList<string, object> data)
        {
            foreach (PairListNode<string, object> element in data)
                if (element.Key.Equals("Name"))
                    name = element.Value.ToString();
                else if (element.Key.Equals("Delay"))
                    delay = int.Parse(element.Value.ToString());
                else if (element.Key.Equals("Quantity"))
                    quantity = int.Parse(element.Value.ToString());
        }

        /// <summary>
        /// ToString override
        /// returns data in a list
        /// </summary>
        /// <returns>list</returns>
        public override string ToString()
        {
            return "{Name:" + name + ",Delay:" + delay + ",Quantity:" + quantity + "}";
        }
    }
}
