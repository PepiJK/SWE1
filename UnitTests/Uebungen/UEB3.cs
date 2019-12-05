using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace Uebungen
{
    public class UEB3
    {
        public void HelloWorld()
        {
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            var req = new Request(network);
            return req;
        }

        public IResponse GetResponse()
        {
            var res = new Response();
            return res;
        }

        public IPlugin GetTestPlugin()
        {
            var pluginManager = new PluginManager();
            return pluginManager.GetSpecificPlugin("TestPlugin");
        }
    }
}
