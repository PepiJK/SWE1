using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    public class Request : IRequest
    {
        private readonly string[] _validRequestMethods =
            {"GET", "HEAD", "POST", "PUT", "DELETE", "CONNECT", "OPTIONS", "TRACE", "PATCH"};

        public bool IsValid { get; }
        public string Method { get; }
        public IUrl Url { get; }
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public string UserAgent => Headers.ContainsKey("user-agent") ? Headers["user-agent"] : null;
        public int HeaderCount { get; }
        public int ContentLength => Headers.ContainsKey("content-length") ? Int32.Parse(Headers["content-length"]) : 0;
        public string ContentType => Headers.ContainsKey("content-type") ? Headers["content-type"] : null;
        public Stream ContentStream => ContentBytes != null ? new MemoryStream(ContentBytes) : null;
        public string ContentString => ContentBytes != null ? Encoding.UTF8.GetString(ContentBytes) : null;
        public byte[] ContentBytes { get; }

        public Request(Stream network)
        {
            IsValid = false;
            ContentBytes = null;

            StreamReader networkReader = new StreamReader(network, Encoding.UTF8);

            if (networkReader.EndOfStream) return;

            string networkLine;

            if ((networkLine = networkReader.ReadLine()) != null)
            {
                string[] reqLine = networkLine.Split(" ");
                if (reqLine.Length == 3)
                {
                    Method = reqLine[0].ToUpper();
                    Url = new Url(reqLine[1]);

                    // method is valid
                    if (_validRequestMethods.Contains(Method))
                    {
                        IsValid = true;

                        // read each line of req till empty line
                        while (true)
                        {
                            networkLine = networkReader.ReadLine().Trim();
                            if (String.IsNullOrEmpty(networkLine)) break;

                            HeaderCount++;
                            string key = networkLine.Split(": ").First().ToLower();
                            string value = networkLine.Split(": ").Last();
                            Headers[key] = value;
                        }

                        // get body of request if content-length in header is set
                        if (ContentLength > 0)
                        {
                            char[] body = new char[ContentLength];
                            networkReader.Read(body, 0, ContentLength);
                            ContentBytes = Encoding.UTF8.GetBytes(body);
                        }
                    }
                }
            }
        }
    }
}