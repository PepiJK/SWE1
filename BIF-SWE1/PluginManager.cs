using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    public class PluginManager : IPluginManager
    {
        private string _pluginPath;
        private List<IPlugin> _plugins = new List<IPlugin>();

        public IEnumerable<IPlugin> Plugins => _plugins;

        public void Add(IPlugin plugin)
        {
            _plugins.Add(plugin);
        }

        public void Add(string plugin)
        {
            var type = Type.GetType(plugin);
            var pluginInstance = (IPlugin) Activator.CreateInstance(type);
            Add(pluginInstance);
        }

        public void Clear()
        {
            _plugins.Clear();
        }

        public PluginManager(string pluginsPath = "../../../../Plugins")
        {
            _pluginPath = pluginsPath;
            
            if (File.Exists(pluginsPath)) throw new Exception("Expected directory, not a file");
            if (!Directory.Exists(pluginsPath))
                throw new Exception("Directory \"" + Path.GetFullPath(pluginsPath) + "\" does not exist");

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

        public IPlugin GetSpecificPlugin(string pluginName)
        {
            string pluginPath = _pluginPath + "/" + pluginName + ".dll";
            Assembly pluginAssembly = LoadPlugin(pluginPath);
            return CreatePlugins(pluginAssembly).First();
        }

        private static Assembly LoadPlugin(string relativePath)
        {
            // Console.WriteLine($"Loading plugins from: {relativePath}");
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