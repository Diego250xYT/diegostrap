using System;

namespace DiegoStrap
{
    internal static class JsonFieldReader
    {
        public static string ReadStringValue(string json, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(fieldName))
            {
                return string.Empty;
            }

            string quotedField = "\"" + fieldName + "\"";
            int fieldIndex = json.IndexOf(quotedField, StringComparison.OrdinalIgnoreCase);
            if (fieldIndex < 0)
            {
                fieldIndex = json.IndexOf(fieldName + ":", StringComparison.OrdinalIgnoreCase);
                if (fieldIndex < 0)
                {
                    return string.Empty;
                }
            }

            int colonIndex = json.IndexOf(':', fieldIndex);
            if (colonIndex < 0)
            {
                return string.Empty;
            }

            int firstQuote = json.IndexOf('"', colonIndex + 1);
            if (firstQuote < 0)
            {
                return string.Empty;
            }

            int secondQuote = json.IndexOf('"', firstQuote + 1);
            if (secondQuote < 0)
            {
                return string.Empty;
            }

            return json.Substring(firstQuote + 1, secondQuote - firstQuote - 1).Trim();
        }
    }
}