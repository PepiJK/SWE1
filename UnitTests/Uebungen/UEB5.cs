using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace Uebungen
{
    public class UEB5
    {
        public void HelloWorld()
        {
        }

        public IPluginManager GetPluginManager()
        {
            IPluginManager pluginManager = new PluginManager();
            pluginManager.Add(new TestPlugin.TestPlugin());
            pluginManager.Add(new StaticFilePlugin.StaticFilePlugin());
            return pluginManager;
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            Request req = new Request(network);
            return req;
        }

        public IPlugin GetStaticFilePlugin()
        {
            var staticFilePlugin = new StaticFilePlugin.StaticFilePlugin() as IPlugin;
            return staticFilePlugin;
        }

        public string GetStaticFileUrl(string fileName)
        {
            Url url = new Url("/" + fileName);
            return url.RawUrl;
        }

        public void SetStatiFileFolder(string folder)
        {
        }
    }
}
