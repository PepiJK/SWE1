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
            {200, "200 OK"}, {400, "400 Bad Request"}, {404, "404 Not Found"}, {420, "420 Requests Too High"},
            {500, "500 Internal Server Error"}, {503, "503 Service Unavailable"}, {501, "501 Not Implemented"}
        };

        public readonly IDictionary<string, string> ValidContentTypes = new Dictionary<string, string>()
        {
            {"html", "text/html; charset=UTF-8"}, {"txt", "text/plain"}, {"css", "text/css"}, {"png", "image/png"},
            {"gif", "image/gif"}, {"jpg", "image/jpeg"}, {"pdf", "application/pdf"}, {"json", "application/json"},
            {"js", "application/javascript"}
        };

        private int StatusCodeIntern { get; set; }
        private byte[] Content { get; set; }

        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public int ContentLength => Content == null ? 0 : Content.Length;
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
            Content = Encoding.UTF8.GetBytes(content);
        }

        public void SetContent(byte[] content)
        {
            Content = content;
        }

        public void SetContent(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                Content = ms.ToArray();
            }
        }

        public void Send(Stream network)
        {
            if (ContentLength == 0 && !String.IsNullOrEmpty(ContentType))
            {
                throw new Exception("ContentType is set but Content is not");
            }

            if (!String.IsNullOrEmpty(Status))
            {
                using (StreamWriter sw = new StreamWriter(network, leaveOpen: true))
                {
                    sw.WriteLine("HTTP/1.1 " + Status);
                    sw.WriteLine("Server: " + ServerHeader);

                    // important to set Connection: close header, otherwise files won't load
                    sw.WriteLine("Connection: close");

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

                    sw.WriteLine();
                }

                if (ContentLength > 0)
                {
                    // need binary writer for images and pdfs
                    using BinaryWriter bw = new BinaryWriter(network, Encoding.UTF8, true);
                    bw.Write(Content, 0, ContentLength);
                }
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
    }
}