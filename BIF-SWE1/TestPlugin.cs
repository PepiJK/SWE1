using System.Linq;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
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
                    Response resp = new Response {StatusCode = 200};
                    resp.SetContent("Valid Request for Segment test or Header test_plugin");
                    return resp;
                }

                if (req.Url.RawUrl == "/")
                {
                    Response resp = new Response {StatusCode = 200};
                    resp.SetContent("Valid Request for Url /");
                    return resp;
                }
            }

            Response res = new Response {StatusCode = 500};
            res.SetContent("Cannot Handle Request");
            return res;
        }
    }
}