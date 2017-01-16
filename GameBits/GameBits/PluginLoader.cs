/*
   Based on PluginLoader by Christoph Gattnar.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

	   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GameBits
{
    /// <summary>
    /// Load assemblies 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class PluginLoader<T>
    {
        private static Type pluginInterfaceType = typeof(T);
        private static ICollection<Type> pluginTypes = new List<Type>();
        private static ICollection<T> plugins = new List<T>();
        private static ICollection<Assembly> assemblies = new List<Assembly>();

        public static ICollection<T> LoadPlugins(string path)
        {
            string[] dllFileNames = null;

            if (Directory.Exists(path))
            {
                dllFileNames = Directory.GetFiles(path, "*.dll");

                foreach (string dllFile in dllFileNames)
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                    Assembly assembly = Assembly.Load(an);
                    if (assembly != null)
                    {
                        // An assembly with the same name as an existing does not replace existing,
                        // therefore the new assembly need only contain new or overridden methods.
                        // Overridden methods will replace existing as types are assigned.
                        assemblies.Add(assembly);
                        Type[] types = assembly.GetTypes();

                        foreach (Type type in types)
                        {
                            if (!type.IsInterface && !type.IsAbstract
                                && type.GetInterface(pluginInterfaceType.FullName) != null)
                            {
                                pluginTypes.Add(type);
                            }
                        }
                    }
                }

                foreach (Type type in pluginTypes)
                {
                    T plugin = (T)Activator.CreateInstance(type);
                    plugins.Add(plugin);
                }

                return plugins;
            }

            return null;
        }

    }
}
