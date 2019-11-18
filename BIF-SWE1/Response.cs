using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    public class Response : IResponse
    {
        private readonly IDictionary<int, string> _validStatusCodes = new Dictionary<int, string>()
        {
            {200, "200 OK"}, {404, "404 Not Found"}, {500, "500 Internal Server Error"}
        };

        private int StatusCodeIntern { get; set; }
        private string Content { get; set; }

        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public int ContentLength { get; set; }
        public string ContentType { get; set; }

        public int StatusCode
        {
            get => StatusCodeIntern == 0
                ? throw new Exception("StatusCode is 0, which means it was probably not set")
                : StatusCodeIntern;
            set => StatusCodeIntern = value;
        }

        public string Status => _validStatusCodes.ContainsKey(StatusCode)
            ? _validStatusCodes[StatusCode]
            : throw new Exception("StatusCode not supported");

        public string ServerHeader { get; set; } = "BIF-SWE1-Server";

        public void SetContent(string content)
        {
            ContentLength = Encoding.UTF8.GetByteCount(content);
            Content = content;
        }

        public void SetContent(byte[] content)
        {
            ContentLength = content.Length;
            Content = Encoding.UTF8.GetString(content);
        }

        public void SetContent(Stream stream)
        {
            ContentLength = (int) stream.Length;
            Content = new StreamReader(stream).ReadToEnd();
        }

        public void Send(Stream network)
        {
            if (String.IsNullOrEmpty(Content) && !String.IsNullOrEmpty(ContentType))
            {
                throw new Exception("ContentType is set but Content is not");
            }

            if (!String.IsNullOrEmpty(Status))
            {
                StreamWriter sw = new StreamWriter(network);
                sw.WriteLine("HTTP/1.1 " + Status);
                sw.WriteLine("Server: " + ServerHeader);

                if (ContentLength > 0)
                {
                    sw.WriteLine("Content-Length: " + ContentLength);
                }

                if (!String.IsNullOrEmpty(ContentType))
                {
                    sw.WriteLine("Content-Type: " + ContentType);
                }

                foreach (var header in Headers)
                {
                    sw.WriteLine(header.Key + ": " + header.Value);
                }

                sw.WriteLine("");

                if (!String.IsNullOrEmpty(Content))
                {
                    sw.Write(Content);
                }

                sw.Flush();
            }
            else
            {
                throw new Exception("Status is not set");
            }
        }

        public void AddHeader(string header, string value)
        {
            if (Headers.ContainsKey(header))
            {
                Headers.Remove(header);
            }

            Headers.Add(header, value);
        }

        public Response()
        {
            ContentLength = 0;
            StatusCodeIntern = 0;
            
            // DebugProperties();
        }

        private void DebugProperties()
        {
            Console.WriteLine("---------DEBUG-RESPONSE----------");
            Console.Write("Headers: ");
            foreach (var head in Headers)
            {
                Console.Write(head);
            }

            Console.WriteLine("\nContentLength: " + ContentLength);
            Console.WriteLine("ContentType" + ContentType);
        }
    }
}