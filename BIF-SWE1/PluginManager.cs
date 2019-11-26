using System.Collections.Generic;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    public class PluginManager : IPluginManager
    {
        private List<IPlugin> _plugins = new List<IPlugin>();

        public IEnumerable<IPlugin> Plugins => _plugins;
        public void Add(IPlugin plugin)
        {
            _plugins.Add(plugin);
        }

        public void Add(string plugin)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            _plugins.Clear();
        }

        public PluginManager()
        {
            TestPlugin plug1 = new TestPlugin();
            Add(plug1);
        }


    }
}