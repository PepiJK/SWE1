using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1;
using NUnit.Framework.Internal;

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
            var testPlugin = new TestPlugin.TestPlugin() as IPlugin;
            return testPlugin;
        }
    }
}
