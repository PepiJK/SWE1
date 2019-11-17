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
        public IDictionary<string, string> Headers { get; }
        public string UserAgent => Headers.ContainsKey("user-agent") ? Headers["user-agent"] : null;
        public int HeaderCount { get; }
        public int ContentLength => Headers.ContainsKey("content-length") ? Int32.Parse(Headers["content-length"]) : 0;
        public string ContentType => Headers.ContainsKey("content-type") ? Headers["content-type"] : null;
        public Stream ContentStream => ContentBytes != null ? new MemoryStream(ContentBytes) : null;
        public string ContentString { get; }
        public byte[] ContentBytes => ContentString != null ? Encoding.UTF8.GetBytes(ContentString) : null;

        public Request(Stream network)
        {
            IsValid = false;
            Headers = new Dictionary<string, string>();
            HeaderCount = 0;
            ContentString = null;

            // network stream is not empty
            if ((int) network.Length > 1)
            {
                // convert data from network stream to string
                StreamReader networkReader = new StreamReader(network);
                string networkString = networkReader.ReadToEnd();

                // get Header Lines and Body from the network String (split between header and body)
                string[] networkHeaderLines = networkString.Split("\n\n").First().Split("\n");
                string networkBody = networkString.Split("\n\n")[1];
                ContentString = networkBody != "" ? networkBody : null;

                // get first line of network stream (the request line) and check if its valid
                string[] reqLine = networkHeaderLines.First().Split(" ");
                if (reqLine.Length == 3)
                {
                    Url = new Url(reqLine[1]);
                    Method = reqLine[0].ToUpper();

                    // method is valid
                    if (_validRequestMethods.Contains(Method))
                    {
                        // get headers form network stream data skip first line was already processed
                        foreach (var line in networkHeaderLines.Skip(1))
                        {
                            HeaderCount++;
                            string key = line.Split(":").First().ToLower();
                            string value = line.Split(":").Last().Trim();
                            Headers[key] = value;
                        }

                        IsValid = true;
                    }
                }
            }

            Console.WriteLine("-----------DEBUG-REQUEST-------");
            Console.WriteLine("IsValid: " + IsValid);
            Console.WriteLine("Method: " + Method);
            Console.Write("Headers: ");
            foreach (var head in Headers)
            {
                Console.Write(head + " ");
            }

            Console.WriteLine("\nUser-Agent: " + UserAgent);
            Console.WriteLine("HeaderCount: " + HeaderCount);
            Console.WriteLine("ContentLength: " + ContentLength);
            Console.WriteLine("ContentType: " + ContentType);
            Console.WriteLine("ContentStream: " + ContentStream);
            Console.WriteLine("ContentString: " + ContentString);
            Console.WriteLine("ContentBytes: " + ContentBytes);
            Console.WriteLine("--------------------------------");
        }
    }
}