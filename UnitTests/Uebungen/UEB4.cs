﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace Uebungen
{
    public class UEB4
    {
        public void HelloWorld()
        {
        }

        public IPluginManager GetPluginManager()
        {
            IPluginManager pluginManager = new PluginManager();
            return pluginManager;
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            IRequest req = new Request(network);
            return req;
        }

        public IResponse GetResponse()
        {
            IResponse res = new Response();
            return res;
        }
    }
}
