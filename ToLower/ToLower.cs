using System;
using System.IO;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace ToLower
{
    public class ToLower : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            if (!req.IsValid) return 0.0f;
            if (req.Url.Path != "/tolower") return 0.0f;
            if (req.Method != "POST") return 0.0f;
            if (req.ContentLength == 0) return 0.0f;
            return 1.0f;
        }

        public IResponse Handle(IRequest req)
        {
            Response resp = new Response();
            resp.StatusCode = 200;

            String[] respString = req.ContentString.Split("=", StringSplitOptions.RemoveEmptyEntries);
            resp.SetContent(respString.Length > 1 ? respString[1].ToLower() : "Bitte geben Sie einen Text ein");
            resp.ContentType = resp.ValidContentTypes["txt"];
            
            return resp;
        }
    }
}