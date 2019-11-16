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
            var url = new Url(path);
            return url;
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
    }
}
