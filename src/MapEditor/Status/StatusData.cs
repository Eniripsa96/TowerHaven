using System;
using System.Collections.Generic;
using System.IO;

using JSON;
using TowerHaven;

namespace WPFEditor
{
    /// <summary>
    /// Status data class
    /// </summary>
    class StatusData
    {
        /// <summary>
        /// Status data files directory
        /// </summary>
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TowerHaven\\Status";
        
        /// <summary>
        /// List of statuses
        /// </summary>
        private static List<Status> statuses;

        /// <summary>
        /// Gets the list of statuses
        /// </summary>
        /// <returns>status name list</returns>
        public static string[] GetStatusNames()
        {
            string[] statusNames = new string[statuses.Count];
            for (int i = 0; i < statuses.Count; ++i)
                statusNames[i] = statuses[i].name;
            return statusNames;
        }

        /// <summary>
        /// Gets the data of the status with the given name
        /// </summary>
        /// <param name="statusName">status name</param>
        /// <returns>status JSON data string</returns>
        public static string GetStatusData(string statusName)
        {
            foreach (Status s in statuses)
                if (s.name.Equals(statusName))
                    return s.ToString();
            return "";
        }

        /// <summary>
        /// Loads status data
        /// </summary>
        public static void Load()
        {
            statuses = new List<Status>();
            try
            {
                using (StreamReader read = new StreamReader(directory))
                {
                    JSONParser parser = new JSONParser(read.ReadToEnd());
                    foreach (PairList<string, object> collection in parser.Objects)
                        foreach (PairListNode<string, object> element in collection)
                            if (element.Key.Equals("Statuses"))
                                foreach (PairList<string, object> status in element.Value as SimpleList<object>)
                                    statuses.Add(new Status(status));
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Saves status data
        /// </summary>
        public static void Save()
        {
            try
            {
                using (StreamWriter save = new StreamWriter(directory))
                {
                    save.Write("{Statuses:[");
                    string data = "";
                    foreach (Status s in statuses)
                        data += s.ToString() + ",";
                    if (data.Length > 0)
                        save.Write(data.Substring(0, data.Length - 1));
                    save.Write("]}");
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Gets the status with the given name
        /// </summary>
        /// <param name="statusName">status name</param>
        /// <returns>status</returns>
        public static Status GetStatus(string statusName)
        {
            foreach (Status s in statuses)
                if (s.name.Equals(statusName))
                    return s;
            return new Status(GetName(), 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0);
        }

        /// <summary>
        /// Updates the status
        /// </summary>
        /// <param name="status">updates status</param>
        public static void UpdateStatus(Status status)
        {
            foreach (Status s in statuses)
                if (s.name.Equals(status.name))
                {
                    s.stunDuration = status.stunDuration;
                    s.slowDuration = status.slowDuration;
                    s.slowMultiplier = status.slowMultiplier;
                    s.slowBonus = status.slowBonus;
                    s.extraDamageDuration = status.extraDamageDuration;
                    s.extraDamageMultiplier = status.extraDamageMultiplier;
                    s.extraDamageBonus = status.extraDamageBonus;
                    s.dotDuration = status.dotDuration;
                    s.dotFrameDamage = status.dotFrameDamage;
                    s.dotMoveDamage = status.dotMoveDamage;
                    return;
                }
            statuses.Add(status);
        }

        /// <summary>
        /// Gets an available status name
        /// </summary>
        /// <returns>available status name</returns>
        private static string GetName()
        {
            string name = "NewStatus1";
            Boolean valid;
            int index = 1;
            do
            {
                valid = true;
                foreach (string s in GetStatusNames())
                    if (name.Equals(s))
                    {
                        name = name.Replace(index + "", ++index + "");
                        valid = false;
                    }
            }
            while (!valid);
            return name;
        }
    }
}
