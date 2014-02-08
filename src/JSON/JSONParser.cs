using System;

using TowerHaven;

namespace JSON
{
    class JSONParser
    {
        /// <summary>
        /// JSON Data
        /// </summary>
        private string data;

        /// <summary>
        /// Object list
        /// </summary>
        private SimpleList<PairList<string, object>> objects;

        /// <summary>
        /// Object list property
        /// </summary>
        public SimpleList<PairList<string, object>> Objects
        {
            get { return objects; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public JSONParser()
        {
            objects = new SimpleList<PairList<string, object>>();
            data = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">JSON data</param>
        public JSONParser(string data)
        {
            objects = new SimpleList<PairList<string, object>>();
            Load(data);
        }

        /// <summary>
        /// Loads the data, erasing all previous data
        /// </summary>
        /// <param name="data">JSON data</param>
        public void Load(string data)
        {
            this.data = data;
            Load();
        }

        /// <summary>
        /// Loads the current data
        /// </summary>
        private void Load()
        {
            objects.Clear();
            while (data.Length > 0)
            {
                char c = data[0];
                data = data.Substring(1);
                if (c == '{')
                {
                    objects.Add(LoadObject());
                }
            }
        }

        /// <summary>
        /// Loads the object from the data
        /// </summary>
        /// <param name="currentObject">object being loaded</param>
        /// <param name="data">JSON data</param>
        private PairList<string, object> LoadObject()
        {
            PairList<string, object> loadedObject = new PairList<string, object>();
            string current = "";
            while (data.Length > 0)
            {
                char c = data[0];
                data = data.Substring(1);

                // End of object
                if (c == '}')
                    return loadedObject;

                // End of key
                else if (c == ':')
                {
                    string key = current;
                    object value = null;
                    current = "";
                    bool repeat = true;
                    while (repeat && data.Length > 0)
                    {
                        c = data[0];
                        data = data.Substring(1);

                        // End of value
                        if (c == ',')
                        {
                            value = current;
                            repeat = false;
                        }

                        // End of value and object
                        else if (c == '}')
                        {
                            value = current;
                            loadedObject.Add(key, value);
                            return loadedObject;
                        }

                        // Value is array
                        else if (c == '[')
                        {
                            value = LoadArray();
                            repeat = false;
                        }

                        // Value is object
                        else if (c == '{')
                        {
                            value = LoadObject();
                            while (data[0] != ',')
                            {
                                if (data[0] == '}')
                                {
                                    data = data.Substring(1);
                                    loadedObject.Add(key, value);
                                    return loadedObject;
                                }
                                data = data.Substring(1);
                            }
                            data = data.Substring(1);
                            repeat = false;

                        }
                        else
                            current += c;
                    }

                    // Add the pair when found
                    loadedObject.Add(key, value);
                    current = "";
                }
                else if (c == ',')
                    continue;
                else
                    current += c;
            }
            return loadedObject;
        }

        /// <summary>
        /// Loads an array from the given data
        /// </summary>
        /// <returns>array</returns>
        private SimpleList<object> LoadArray()
        {
            SimpleList<object> array = new SimpleList<object>();

            // Empty array
            if (data[0] == ']')
            {
                data = data.Substring(1);
                return array;
            }

            string current = "";
            while (data.Length > 0)
            {
                char c = data[0];
                data = data.Substring(1);

                // End of element
                if (c == ',')
                {
                    array.Add(current);
                    current = "";
                }

                // End of array
                else if (c == ']')
                {
                    array.Add(current);
                    return array;
                }

                // Array inside the array
                else if (c == '[')
                {
                    array.Add(LoadArray());
                    if (data[0] != ',' && data[0] != ']')
                        throw new InvalidJSONDataException("Illegal character after ']': '" + data[0] + "'.");
                    if (data[0] == ',')
                        data = data.Substring(1);
                    if (data[0] == ']')
                    {
                        data = data.Substring(1);
                        return array;
                    }
                }

                // Object inside the array
                else if (c == '{')
                {
                    array.Add(LoadObject());
                    if (data[0] != ',' && data[0] != ']')
                        throw new InvalidJSONDataException("Illegal character after '}' in array: '" + data[0] + "'.");
                    if (data[0] == ',')
                        data = data.Substring(1);
                    if (data[0] == ']')
                    {
                        data = data.Substring(1);
                        return array;
                    }
                }
                else
                {
                    current += c;
                }
            }
            return array;
        }
    }
}
