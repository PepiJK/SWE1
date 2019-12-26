using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using BIF.SWE1.Interfaces;
using NUnit.Framework;

namespace BIF.SWE1.UnitTests
{
    public class RespObj
    {
        public string msg { get; set; }
        public string[] cities { get; set; }
    }

    [TestFixture]
    public class NaviPluginTests : AbstractTestFixture<Uebungen.UEB6>
    {
        private static StringBuilder GetBody(IResponse resp)
        {
            StringBuilder body = new StringBuilder();
            using (var ms = new MemoryStream())
            {
                resp.Send(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sr = new StreamReader(ms);
                while (!sr.EndOfStream)
                {
                    body.AppendLine(sr.ReadLine());
                }
            }

            return body;
        }

        [Test]
        public void navi_plugin_contains_summary_custom()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPlugin returned null");

            // wait 1s for the map being loaded (smaller version for unit tests)
            Thread.Sleep(1000);

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(
                RequestHelper.GetValidRequestStream(url, method: "POST", body: "street=Hauptplatz"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));

            var body = GetBody(resp);
            Assert.That(body.ToString(), Does.Contain("Orte gefunden")); // 42 Orte gefunden
        }

        [Test]
        public void navi_plugin_handle_custom()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPlugin returned null");

            // wait 1s for the map being loaded (smaller version for unit tests)
            Thread.Sleep(1000);

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(
                RequestHelper.GetValidRequestStream(url, method: "POST", body: "street=Hauptplatz"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));
        }

        [Test]
        public void navi_plugin_handle_empty_custom()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPlugin returned null");

            // wait 1s for the map being loaded (smaller version for unit tests)
            Thread.Sleep(1000);

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url, method: "POST", body: "street="));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));

            var body = GetBody(resp);
            Assert.That(body.ToString(), Does.Contain("Bitte geben Sie eine Anfrage ein"));
        }

        [Test]
        public void navi_plugin_return_503_while_busy()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPlugin returned null");

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(
                RequestHelper.GetValidRequestStream(url, method: "POST", body: "street=Hauptplatz"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(503));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));
        }

        [Test]
        public void navi_plugin_refresh_map()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPlugin returned null");

            // wait 1s for the map being loaded (smaller version for unit tests)
            Thread.Sleep(1000);

            var url = ueb.GetNaviUrl() + "?refresh=1";
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");
            var req = ueb.GetRequest(RequestHelper.GetValidRequestStream(url));

            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));

            // no wait -> plugin must be busy

            url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");
            req = ueb.GetRequest(
                RequestHelper.GetValidRequestStream(url, method: "POST", body: "street=Hauptplatz"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(503));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));
        }

        [Test]
        public void navi_plugin_return_cities()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPlugin returned null");

            // wait 1s for the map being loaded (smaller version for unit tests)
            Thread.Sleep(1000);

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(
                RequestHelper.GetValidRequestStream(url, method: "POST", body: "street=Leopold-Ferstl-Gasse"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));

            Assert.That(resp.ContentType, Is.EqualTo("application/json"));
            var body = GetBody(resp).ToString().Split("\n", StringSplitOptions.RemoveEmptyEntries).Last();
            var json = JsonSerializer.Deserialize<RespObj>(body);
            Assert.IsNotNull(json);
            Assert.That(json.msg, Is.EqualTo("1 Orte gefunden"));
            Assert.That(json.cities.Length, Is.EqualTo(1));
            Assert.That(json.cities.First(), Is.EqualTo("Wien"));
        }
        
        [Test]
        public void navi_plugin_return_no_cities()
        {
            var ueb = CreateInstance();
            var plugin = ueb.GetNavigationPlugin();
            Assert.That(plugin, Is.Not.Null, "IUEB6.GetNavigationPlugin returned null");

            // wait 1s for the map being loaded (smaller version for unit tests)
            Thread.Sleep(1000);

            var url = ueb.GetNaviUrl();
            Assert.That(url, Is.Not.Null, "IUEB6.GetNaviUrl returned null");

            var req = ueb.GetRequest(
                RequestHelper.GetValidRequestStream(url, method: "POST", body: "street=asdf"));
            Assert.That(req, Is.Not.Null, "IUEB6.GetRequest returned null");

            Assert.That(plugin.CanHandle(req), Is.GreaterThan(0).And.LessThanOrEqualTo(1));

            var resp = plugin.Handle(req);
            Assert.That(resp, Is.Not.Null);
            Assert.That(resp.StatusCode, Is.EqualTo(200));
            Assert.That(resp.ContentLength, Is.GreaterThan(0));

            Assert.That(resp.ContentType, Is.EqualTo("application/json"));
            var body = GetBody(resp).ToString().Split("\n", StringSplitOptions.RemoveEmptyEntries).Last();
            var json = JsonSerializer.Deserialize<RespObj>(body);
            Assert.IsNotNull(json);
            Assert.That(json.msg, Is.EqualTo("0 Orte gefunden"));
            Assert.That(json.cities, Is.Null);
        }
    }
}