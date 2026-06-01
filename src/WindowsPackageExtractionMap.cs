using System;
using System.Collections.Generic;

namespace DiegoStrap
{
    internal static class WindowsPackageExtractionMap
    {
        private static readonly Dictionary<string, Dictionary<string, string>> Roots = CreateRoots();

        public static string GetRoot(string binaryType, string packageName)
        {
            if (binaryType == null)
            {
                return string.Empty;
            }

            if (packageName == null)
            {
                return string.Empty;
            }

            if (!Roots.ContainsKey(binaryType))
            {
                return string.Empty;
            }

            Dictionary<string, string> roots = Roots[binaryType];

            if (!roots.ContainsKey(packageName))
            {
                return string.Empty;
            }

            return roots[packageName];
        }

        private static Dictionary<string, Dictionary<string, string>> CreateRoots()
        {
            Dictionary<string, Dictionary<string, string>> roots = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            Dictionary<string, string> player = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["RobloxApp.zip"] = string.Empty,
                ["redist.zip"] = string.Empty,
                ["shaders.zip"] = "shaders/",
                ["ssl.zip"] = "ssl/",
                ["content-avatar.zip"] = "content/avatar/",
                ["content-configs.zip"] = "content/configs/",
                ["content-fonts.zip"] = "content/fonts/",
                ["content-sky.zip"] = "content/sky/",
                ["content-sounds.zip"] = "content/sounds/",
                ["content-textures2.zip"] = "content/textures/",
                ["content-models.zip"] = "content/models/",
                ["content-platform-fonts.zip"] = "PlatformContent/pc/fonts/",
                ["content-platform-dictionaries.zip"] = "PlatformContent/pc/shared_compression_dictionaries/",
                ["content-terrain.zip"] = "PlatformContent/pc/terrain/",
                ["content-textures3.zip"] = "PlatformContent/pc/textures/",
                ["extracontent-luapackages.zip"] = "ExtraContent/LuaPackages/",
                ["extracontent-translations.zip"] = "ExtraContent/translations/",
                ["extracontent-models.zip"] = "ExtraContent/models/",
                ["extracontent-textures.zip"] = "ExtraContent/textures/",
                ["extracontent-places.zip"] = "ExtraContent/places/"
            };

            Dictionary<string, string> studio = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["RobloxStudio.zip"] = string.Empty,
                ["RibbonConfig.zip"] = "RibbonConfig/",
                ["redist.zip"] = string.Empty,
                ["Libraries.zip"] = string.Empty,
                ["LibrariesQt5.zip"] = string.Empty,
                ["WebView2.zip"] = string.Empty,
                ["WebView2RuntimeInstaller.zip"] = string.Empty,
                ["shaders.zip"] = "shaders/",
                ["ssl.zip"] = "ssl/",
                ["Qml.zip"] = "Qml/",
                ["Plugins.zip"] = "Plugins/",
                ["StudioFonts.zip"] = "StudioFonts/",
                ["BuiltInPlugins.zip"] = "BuiltInPlugins/",
                ["ApplicationConfig.zip"] = "ApplicationConfig/",
                ["BuiltInStandalonePlugins.zip"] = "BuiltInStandalonePlugins/",
                ["content-qt_translations.zip"] = "content/qt_translations/",
                ["content-sky.zip"] = "content/sky/",
                ["content-fonts.zip"] = "content/fonts/",
                ["content-avatar.zip"] = "content/avatar/",
                ["content-models.zip"] = "content/models/",
                ["content-sounds.zip"] = "content/sounds/",
                ["content-configs.zip"] = "content/configs/",
                ["content-api-docs.zip"] = "content/api_docs/",
                ["content-textures2.zip"] = "content/textures/",
                ["content-studio_svg_textures.zip"] = "content/studio_svg_textures/",
                ["content-platform-fonts.zip"] = "PlatformContent/pc/fonts/",
                ["content-platform-dictionaries.zip"] = "PlatformContent/pc/shared_compression_dictionaries/",
                ["content-terrain.zip"] = "PlatformContent/pc/terrain/",
                ["content-textures3.zip"] = "PlatformContent/pc/textures/",
                ["extracontent-translations.zip"] = "ExtraContent/translations/",
                ["extracontent-luapackages.zip"] = "ExtraContent/LuaPackages/",
                ["extracontent-textures.zip"] = "ExtraContent/textures/",
                ["extracontent-scripts.zip"] = "ExtraContent/scripts/",
                ["extracontent-models.zip"] = "ExtraContent/models/",
                ["studiocontent-models.zip"] = "StudioContent/models/",
                ["studiocontent-textures.zip"] = "StudioContent/textures/"
            };

            roots["WindowsPlayer"] = player;
            roots["WindowsStudio64"] = studio;

            return roots;
        }
    }
}