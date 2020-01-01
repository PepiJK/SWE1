using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace StaticFilePlugin
{
    /// <summary>
    /// StaticFilePlugin class that processes a request and returns a file.
    /// </summary>
    public class StaticFilePlugin : IPlugin
    {
        private const string StaticFilesPath = "./static-files";

        public float CanHandle(IRequest req)
        {
            if (!req.IsValid) return 0.0f;
            if (String.IsNullOrEmpty(req.Url.FileName) || String.IsNullOrEmpty(req.Url.FileName)) return 0.0f;
            if (!File.Exists(StaticFilesPath + req.Url.Path)) return 0.1f;
            return 0.2f;
        }

        public IResponse Handle(IRequest req)
        {
            Response resp = new Response();
            resp.StatusCode = 500;
            resp.SetContent(resp.Status);
            resp.ContentType = resp.ValidContentTypes["txt"];

            if (req.IsValid)
            {
                if (!String.IsNullOrEmpty(req.Url.FileName) && !String.IsNullOrEmpty(req.Url.Extension))
                {
                    resp.StatusCode = 404;
                    resp.SetContent(resp.Status);
                    resp.ContentType = resp.ValidContentTypes["txt"];

                    if (File.Exists(StaticFilesPath + req.Url.Path))
                    {
                        string extension = Path.GetExtension(StaticFilesPath + req.Url.Path).Trim('.');
                        if (resp.ValidContentTypes.ContainsKey(extension))
                        {
                            Console.WriteLine("set content, file: " + req.Url.Path);
                            resp.StatusCode = 200;
                            resp.SetContent(File.OpenRead(StaticFilesPath + req.Url.Path));
                            resp.ContentType = resp.ValidContentTypes[extension];
                        }
                        else
                        {
                            resp.StatusCode = 501;
                            resp.SetContent("Content Type is not supported!");
                            resp.ContentType = resp.ValidContentTypes["txt"];
                        }
                    }
                }
            }

            return resp;
        }
    }
}