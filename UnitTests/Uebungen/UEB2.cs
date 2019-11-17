using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace Uebungen
{
    public class UEB2
    {
        public void HelloWorld()
        {
        }

        public IUrl GetUrl(string path)
        {
            IUrl url = new Url(path);
            return url;
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
