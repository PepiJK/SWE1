using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    /// <summary>
    /// PluginManager class that handles plugins which are loaded via assemblies from a directory.
    /// </summary>
    public class PluginManager : IPluginManager
    {
        private readonly List<IPlugin> _plugins = new List<IPlugin>();

        public IEnumerable<IPlugin> Plugins => _plugins;

        public void Add(IPlugin plugin)
        {
            _plugins.Add(plugin);
        }

        public void Add(string plugin)
        {
            var type = Type.GetType(plugin);
            var pluginInstance =
                (IPlugin) Activator.CreateInstance(type ?? throw new NullReferenceException("plugin type is null"));
            Add(pluginInstance);
        }

        public void Clear()
        {
            _plugins.Clear();
        }

        /// <summary>
        /// Loads Plugins from directory path and add it to Plugins list.
        /// </summary>
        /// <param name="pluginsPath"></param>
        /// <exception cref="Exception"></exception>
        public PluginManager(string pluginsPath = "./plugins")
        {
            if (File.Exists(pluginsPath)) throw new Exception("Expected directory, not a file");
            if (!Directory.Exists(pluginsPath))
                throw new DirectoryNotFoundException();

            // load each dll file in Plugin Directory, create plugin, add it to Plugins List
            string[] fileEntries = Directory.GetFiles(pluginsPath, "*.dll");
            IEnumerable<IPlugin> plugins = fileEntries.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreatePlugins(pluginAssembly);
            });

            foreach (var plugin in plugins)
            {
                Add(plugin);
            }
        }

        private static Assembly LoadPlugin(string relativePath)
        {
            PluginLoadContext loadContext = new PluginLoadContext(relativePath);
            if (File.Exists(relativePath))
            {
                return loadContext.LoadFromAssemblyName(
                    new AssemblyName(Path.GetFileNameWithoutExtension(relativePath)));
            }

            return null;
        }

        private static IEnumerable<IPlugin> CreatePlugins(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    if (Activator.CreateInstance(type) is IPlugin result)
                    {
                        yield return result;
                    }
                }
            }
        }
    }
}