using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    class Program
    {
        private const int Port = 8080;
        private static readonly PluginManager PluginManager = new PluginManager();

        static void Main()
        {
            Console.WriteLine("BIF-SWE1 WebServer");
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            Console.WriteLine("Listening on Port " + Port + "...");
            listener.Start();

            while (true)
            {
                Socket s = listener.AcceptSocket();
                Thread thread = new Thread(() => Listen(s));
                thread.Start();
            }
        }

        private static void Listen(Socket s)
        {
            Console.WriteLine("New Client connected");
            using NetworkStream stream = new NetworkStream(s);
            
            Response res;
            try
            {
                Request req = new Request(stream);
                IPlugin selectedPlugin = null;
                float maxScore = 0.0f;
                    
                foreach (var plugin in PluginManager.Plugins)
                {
                    var score = plugin.CanHandle(req);
                    if (score > maxScore)
                    {
                        maxScore = score;
                        selectedPlugin = plugin;
                    }
                }

                if (selectedPlugin != null)
                {
                    res = selectedPlugin.Handle(req) as Response;
                    if (res != null)
                    {
                        res.Send(stream);
                    }
                    else
                    {
                        res = new Response {StatusCode = 500};
                        res.ContentType = res.ValidContentTypes["txt"];
                        res.SetContent(res.Status);
                        res.Send(stream);
                    }
                }
                else
                {
                    res = new Response {StatusCode = 501};
                    res.ContentType = res.ValidContentTypes["txt"];
                    res.SetContent(res.Status);
                    res.Send(stream);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                res = new Response {StatusCode = 500};
                res.ContentType = res.ValidContentTypes["txt"];
                res.Send(stream);
            }

            Console.WriteLine("Status: " + res.Status);
        }
    }
}