using System.Globalization;
using UnityEngine;

namespace Tools
{
    public static class ParseHelper
    {
        public static bool TryParseFloat(string stringToParse, out float value)
        {
            if (float.TryParse(stringToParse, NumberStyles.Any, CultureInfo.InvariantCulture,
                    out var parsedValue))
            {
                value = parsedValue;
                return true;
            }
            
            Debug.LogError($"Failed to parse float string: {stringToParse}");
            value = default;
            return false;
        }
        
        public static bool TryParseInt(string stringToParse, out int value)
        {
            if (int.TryParse(stringToParse, NumberStyles.Any, CultureInfo.InvariantCulture,
                    out var parsedValue))
            {
                value = parsedValue;
                return true;
            }
            
            Debug.LogError($"Failed to parse int string: {stringToParse}");
            value = default;
            return false;
        }
    }
}