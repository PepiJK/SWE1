using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    class Program
    {
        private const int Port = 8080;

        static void Main(string[] args)
        {
            Console.WriteLine("BIF-SWE1 WebServer");
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            Console.WriteLine("Listening on Port " + Port + "...");
            listener.Start();

            while (true)
            {
                Socket s = listener.AcceptSocket();
                PluginManager pluginManager = new PluginManager();
                Thread thread = new Thread(() => Listen(s, pluginManager));
                thread.Start();
            }
        }

        private static void Listen(Socket s, PluginManager pluginManager)
        {
            Console.WriteLine("New Client connected");

            using (NetworkStream stream = new NetworkStream(s))
            {
                Response res = null;
                try
                {
                    Request req = new Request(stream);

                    if (!req.IsValid)
                    {
                        res = new Response {StatusCode = 400};
                        res.ContentType = "text/html; charset=UTF-8";
                        res.SetContent(res.Status);
                        res.Send(stream);
                    }
                    else
                    {
                        IPlugin selectedPlugin = null;
                        float maxScore = 0.0f;
                        foreach (var plugin in pluginManager.Plugins)
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
                                res.ContentType = "text/html; charset=UTF-8";
                                res.SetContent(res.Status);
                                res.Send(stream);
                            }
                        }
                        else
                        {
                            res = new Response {StatusCode = 501};
                            res.ContentType = "text/html; charset=UTF-8";
                            res.SetContent(res.Status);
                            res.Send(stream);
                        }
                    }
                }

                catch (Exception e)
                {
                    res = new Response {StatusCode = 500};
                    res.ContentType = "text/html; charset=UTF-8";
                    res.SetContent("<pre>" + e + "</pre>");
                    res.Send(stream);
                }

                Console.WriteLine("Status: " + res.Status);
                stream.Close();
            }
        }
    }
}