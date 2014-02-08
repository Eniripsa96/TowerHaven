using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowerHaven
{
    /// <summary>
    /// Handles beeping for the game
    /// </summary>
    class Beep
    {
        /// <summary>
        /// Tells when to beep or not
        /// </summary>
        public static bool shouldBeep;

        /// <summary>
        /// Thread method
        /// Loops infinitely, beeping when needed
        /// </summary>
        public static void Run()
        {
            while (true)
            {
                if (shouldBeep)
                {
                    Console.Beep();
                    shouldBeep = false;
                }
            }
        }
    }
}
