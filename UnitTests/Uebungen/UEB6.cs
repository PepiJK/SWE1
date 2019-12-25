using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using BIF_SWE1;
using TempPlugin;

namespace Uebungen
{
    public class UEB6
    {
        public void HelloWorld()
        {
        }

        public IPluginManager GetPluginManager()
        {
            IPluginManager pluginManager = new PluginManager();
            pluginManager.Add(new TestPlugin.TestPlugin());
            pluginManager.Add(new StaticFilePlugin.StaticFilePlugin());
            pluginManager.Add(new ToLower.ToLower());
            return pluginManager;
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            Request req = new Request(network);
            return req;
        }

        public string GetNaviUrl()
        {
            throw new NotImplementedException();
        }

        public IPlugin GetNavigationPlugin()
        {
            throw new NotImplementedException();
        }

        public IPlugin GetTemperaturePlugin()
        {
            var tempPlugin = new TempPlugin.TempPlugin() as IPlugin;
            return tempPlugin;
        }

        public string GetTemperatureRestUrl(DateTime from, DateTime until)
        {
            string fromString = from.Year + "-" + from.Month + "-" + from.Day;
            string untilString = until.Year + "-" + until.Month + "-" + until.Day;
            return "/temperature/json/" + fromString + "/" + untilString;
        }

        public string GetTemperatureUrl(DateTime from, DateTime until)
        {
            string fromString = from.Year + "-" + from.Month + "-" + from.Day;
            string untilString = until.Year + "-" + until.Month + "-" + until.Day;
            return "/temperature/html/" + fromString + "/" + untilString;
        }

        public IPlugin GetToLowerPlugin()
        {
            var toLowerPlugin = new ToLower.ToLower() as IPlugin;
            return toLowerPlugin;
        }

        public string GetToLowerUrl()
        {
            return "/tolower";
        }
    }
}
