using Microsoft.Extensions.Configuration;
using System.IO;

namespace Utils
{
    public static class AppConfiguration
    {
        public static string GetAppsetting(string section, params string[] childrensection)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, true, true);
            var root = configurationBuilder.Build();
            var value = root.GetSection(section);
            foreach (var child in childrensection)
            {
                value = value.GetSection(child);
            }
            return value.Value;
        }
    }
}