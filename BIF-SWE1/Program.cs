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
            Console.WriteLine($"Listening on Port " + Port + "...");
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
            using (NetworkStream stream = new NetworkStream(s))
            {
                Request req = new Request(stream);
                Response res = new Response();

                if (req.IsValid)
                {
                    res.ContentType = "text/html; charset=UTF-8";
                    res.SetContent("<html><body><h1>200 SUCCESS Request is Valid</h1></body></html>");
                    res.StatusCode = 200;
                    res.Send(stream);
                }
                else
                {
                    res.ContentType = "text/html; charset=UTF-8";
                    res.SetContent("<html><body><h1>500 ERROR Request not Valid</h1></body></html>");
                    res.StatusCode = 500;
                    res.Send(stream);
                }

                Console.WriteLine("Status: " + res.StatusCode);
                stream.Close();
            }
        }
    }
}