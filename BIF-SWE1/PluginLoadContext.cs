using System;
using System.Reflection;
using System.Runtime.Loader;

namespace BIF_SWE1
{
    /// <summary>
    /// PluginLoadContext class that is handling assemblies.
    /// Create a custom AssemblyLoadContext to load each plugin.
    /// Use the System.Runtime.Loader.AssemblyDependencyResolver type to allow plugins to have dependencies.
    /// Code used from https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support.
    /// Dotnet Core 3.0 or newer required.
    /// </summary>
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        /// <summary>
        /// Create AssemblyDependencyResolver
        /// </summary>
        /// <param name="pluginPath"></param>
        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}