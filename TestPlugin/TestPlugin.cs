using System;
using BIF.SWE1.Interfaces;
using BIF_SWE1;

namespace TestPlugin
{
    public class TestPlugin : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            if (req.IsValid)
            {
                if ((req.Url.Segments.Length > 0 && req.Url.Segments[0] == "test") ||
                    req.Url.Parameter.ContainsKey("test_plugin"))
                {
                    return 1.0f;
                }

                if (req.Url.RawUrl == "/")
                {
                    return 0.1f;
                }
            }

            return 0.0f;
        }

        public IResponse Handle(IRequest req)
        {
            if (req.IsValid)
            {
                if ((req.Url.Segments.Length > 0 && req.Url.Segments[0] == "test") ||
                    req.Url.Parameter.ContainsKey("test_plugin"))
                {
                    var resp = new Response {StatusCode = 200};
                    resp.SetContent("Valid Request for Segment test or Parameter Key test_plugin");
                    resp.ContentType = resp.ValidContentTypes["txt"];
                    return resp;
                }

                if (req.Url.RawUrl == "/")
                {
                    var resp = new Response {StatusCode = 200};
                    resp.SetContent("Valid Request for Url /");
                    resp.ContentType = resp.ValidContentTypes["txt"];
                    return resp;
                }
            }

            var res = new Response {StatusCode = 500};
            res.SetContent("TestPlugin Cannot Handle Request");
            res.ContentType = res.ValidContentTypes["txt"];
            return res;
        }
    }
}