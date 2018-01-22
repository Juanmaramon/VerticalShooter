using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Persistence. Save values on player prefs
    /// </summary>
    public class Persistence
    {
        /// <summary>
        /// Sets the max value on persistence.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="newValue">New value.</param>
        public static void SetMaxValue(string key, int newValue)
        {
            if (PlayerPrefs.HasKey(key) && newValue > PlayerPrefs.GetInt(key))
            {
                PlayerPrefs.SetInt(key, newValue);
            }
            else if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, newValue);
            }
        }

        /// <summary>
        /// Returns the value saved on persistence.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="key">Key.</param>
        public static int ReturnValue(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key);
            }

            return 0;
        }
    }
}