using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace Uebungen
{
    public class UEB1
    {
        public void HelloWorld()
        {
            // I'm fine
        }

        public IUrl GetUrl(string path)
        {
            var url = new Url(path);
            return url;
        }
    }
}
