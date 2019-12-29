using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace NaviPlugin
{
    public class NaviPlugin : IPlugin
    {
        private const string Url = "/navi";
        private readonly Dictionary<string, List<string>> _streetCities = new Dictionary<string, List<string>>();
        private bool _loadingMap;
        private readonly Mutex _mtx = new Mutex();

        public float CanHandle(IRequest req)
        {
            if (!req.IsValid) return 0.0f;
            if (req.Url.Path == Url) return 1.0f;
            return 0.0f;
        }

        public IResponse Handle(IRequest req)
        {
            var resp = new Response();

            if (_loadingMap)
            {
                // plugin is busy while loading the map
                resp.StatusCode = 503;
                resp.SetContent(resp.Status);
                resp.ContentType = resp.ValidContentTypes["txt"];
            }
            else
            {
                if (req.Method == "GET" && req.Url.Parameter.ContainsKey("refresh") &&
                    req.Url.Parameter["refresh"] == "1")
                {
                    // reload the map
                    Thread t = new Thread(LoadMap);
                    t.Start();
                    resp.StatusCode = 200;
                    resp.SetContent(resp.Status);
                    resp.ContentType = resp.ValidContentTypes["txt"];
                }
                else if (req.Method == "POST" && req.ContentLength > 0)
                {
                    // return the cities of the provided street
                    string key = req.ContentString.Split("=").First();
                    string value = RemoveInvalidChars(req.ContentString.Split("=").Last());

                    if (key == "street")
                    {
                        if (String.IsNullOrEmpty(value))
                        {
                            resp.SetContent("{\"msg\": \"Bitte geben Sie eine Anfrage ein\"}");
                        }
                        else
                        {
                            int amountOfCities = _streetCities.ContainsKey(value) ? _streetCities[value].Count : 0;
                            resp.SetContent("{\"msg\": \"" + amountOfCities + " Orte gefunden\"}");

                            if (amountOfCities > 0)
                            {
                                var citiesJson = JsonSerializer.Serialize(_streetCities[value]);
                                resp.SetContent("{\"msg\": \"" + amountOfCities + " Orte gefunden\", \"cities\": " +
                                                citiesJson + "}");
                            }
                        }

                        resp.StatusCode = 200;
                        resp.ContentType = resp.ValidContentTypes["json"];
                    }
                }
                else
                {
                    resp.StatusCode = 400;
                    resp.SetContent(resp.Status);
                    resp.ContentType = resp.ValidContentTypes["txt"];
                }
            }

            return resp;
        }

        public NaviPlugin()
        {
            Thread t = new Thread(LoadMap);
            t.Start();
        }

        private void LoadMap()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            lock (_mtx)
            {
                _loadingMap = true;
                Console.WriteLine("Load map...");
                var file = "./navi/data.osm";
                using (var fs = File.OpenRead(file))
                {
                    using var xmlReader = XmlReader.Create(fs, settings);
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "osm")
                        {
                            using var osm = xmlReader.ReadSubtree();
                            while (osm.Read())
                            {
                                if (osm.NodeType == XmlNodeType.Element &&
                                    (osm.Name == "node" || osm.Name == "way"))
                                {
                                    string city = "";
                                    string street = "";

                                    using (var element = osm.ReadSubtree())
                                    {
                                        while (element.Read())
                                        {
                                            if (element.NodeType == XmlNodeType.Element
                                                && element.Name == "tag")
                                            {
                                                string tagType = element.GetAttribute("k");
                                                string value = element.GetAttribute("v");

                                                if (tagType == "addr:city") city = value;
                                                if (tagType == "addr:street") street = value;
                                            }
                                        }
                                    }

                                    if (city != "")
                                    {
                                        if (_streetCities.ContainsKey(street))
                                        {
                                            if (!_streetCities[street].Contains(city))
                                            {
                                                _streetCities[street].Add(city);
                                            }
                                        }
                                        else
                                        {
                                            _streetCities.Add(street, new List<string> {city});
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Finished loading!");
                _loadingMap = false;
            }
        }

        private string RemoveInvalidChars(string input)
        {
            // replace invalid chars with empty strings
            // needed for special chars like ß (ContentString with ß also contains a square?)
            try
            {
                return Regex.Replace(input, @"[^ \w\.@-]", "",
                    RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // return empty string when regex timed out
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
    }
}