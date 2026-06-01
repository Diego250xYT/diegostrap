using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DiegoStrap
{
    internal static class AccountStore
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public static string StorePath
        {
            get
            {
                return Path.Combine(AppContext.BaseDirectory, "accounts.json");
            }
        }

        public static List<string> LoadUsernames()
        {
            try
            {
                if (!File.Exists(StorePath))
                {
                    return new List<string>();
                }

                string json = File.ReadAllText(StorePath);
                List<string> usernames = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                usernames.RemoveAll(string.IsNullOrWhiteSpace);
                return usernames;
            }
            catch
            {
                return new List<string>();
            }
        }

        public static void SaveUsernames(IEnumerable<string> usernames)
        {
            string json = JsonSerializer.Serialize(usernames, JsonOptions);
            File.WriteAllText(StorePath, json);
        }
    }
}