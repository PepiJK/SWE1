using System;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace TempPlugin
{
    public class TempPlugin : IPlugin
    {
        private readonly string _Url = "/temperature";
        private TempController _tempController = new TempController();

        public float CanHandle(IRequest req)
        {
            if (!req.IsValid) return 0.0f;
            if (req.Method != "GET") return 0.0f;
            if (!req.Url.Path.Contains(_Url)) return 0.0f;
            if (req.Url.Segments.Length < 2) return 0.0f;
            if (req.Url.Segments[1] == "html") return 0.9f;
            if (req.Url.Segments[1] == "json") return 1.0f;
            return 0.0f;
        }

        public IResponse Handle(IRequest req)
        {
            var resp = new Response();
            resp.StatusCode = 400;
            resp.SetContent(resp.Status);
            resp.ContentType = resp.ValidContentTypes["txt"];

            if (req.Url.Segments[1] == "html")
            {
                // html temp plugin is handled via the static file plugin (returns the temperature.html)
                resp.StatusCode = 200;
                resp.SetContent(resp.Status);
                resp.ContentType = resp.ValidContentTypes["html"];
            }
            else if (req.Url.Segments[1] == "json")
            {
                int pageIndex, pageSize;
                
                try
                {
                    pageIndex = Int16.Parse(req.Url.Parameter["pageindex"]);
                }
                catch (Exception)
                {
                    pageIndex = 1;
                }
                try
                {
                    pageSize = Int16.Parse(req.Url.Parameter["pagesize"]);
                }
                catch (Exception)
                {
                    pageSize = 20;
                }

                var tempController = new TempController();
                TempPaginatedList temps;
                
                if (req.Url.Segments.Length == 3)
                {
                    DateTime date;
                    
                    try
                    {
                        date = DateTime.Parse(req.Url.Segments[2]);
                    }
                    catch (Exception)
                    {
                        date = DateTime.Today;
                    }
                    
                    temps = tempController.GetTempsByDateAsPaginatedList(date, pageIndex, pageSize);
                }
                else
                {
                    temps = tempController.GetTempsAsPaginatedList(pageIndex, pageSize);
                }

                var tempsJson = JsonSerializer.Serialize(temps);

                resp.StatusCode = 200;
                resp.ContentType = resp.ValidContentTypes["json"];
                resp.SetContent(tempsJson);
            }

            return resp;
        }

        public TempPlugin()
        {
            Thread t = new Thread(ReadSensor);
            t.Start();
        }

        private void ReadSensor()
        {
            while (true)
            {
                // add a new row every 5 seconds
                Thread.Sleep(5000);

                // random.NextDouble() * (maximum - minimum) + minimum;
                // generate random float ranging form -10 to 25
                var random = new Random();
                var randomFloat = (float) random.NextDouble() * (25 - (-10)) + (-10);

                _tempController.AddTemp(new TempModel
                {
                    DateTime = DateTime.Now,
                    Value = randomFloat
                });
            }
        }
    }
}