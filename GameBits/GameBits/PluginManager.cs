using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GameBits
{
    public static class PluginManager
    {
        static Dictionary<string, IPlugin> _plugins = new Dictionary<string, IPlugin>();

        /// <summary>
        /// Remove all loaded plugins.
        /// </summary>
        public static void Clear()
        {
            _plugins.Clear();
        }

        /// <summary>
        /// Load all plugins from the specified folder (non-recursive) without removing any already loaded, but replacing any that have the same name.
        /// </summary>
        /// <param name="FolderPath"></param>
        public static void Load(string FolderPath)
        {
            ICollection<IPlugin> list = PluginLoader<IPlugin>.LoadPlugins(FolderPath);
            foreach (var item in list)
            {
                _plugins[item.Name] = item;
            }
        }

        /// <summary>
        /// Return true if the specified plugin is loaded.
        /// </summary>
        /// <param name="PluginName"></param>
        /// <returns></returns>
        public static bool IsLoaded(string PluginName)
        {
            return (_plugins.ContainsKey(PluginName));
        }

        /// <summary>
        /// Get a plugin if loaded, else null.
        /// </summary>
        /// <param name="PluginName"></param>
        /// <returns></returns>
        public static IPlugin Get(string PluginName)
        {
            if (IsLoaded(PluginName))
            {
                return _plugins[PluginName];
            }
            else
            {
                return null;
            }
        }
    }
}
