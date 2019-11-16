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
        public bool IsValid { get; }
        public string Method { get; }
        public IUrl Url { get; }
        public IDictionary<string, string> Headers { get; }
        public string UserAgent { get; }
        public int HeaderCount { get; }
        public int ContentLength { get; }

        public string ContentType { get; }

        public Stream ContentStream { get; }
        public string ContentString { get; }
        public byte[] ContentBytes { get; }
        
        private readonly string[] _validRequestMethods =
        {
            "GET", "HEAD", "POST", "PUT", "DELETE", "CONNECT", "OPTIONS", "TRACE", "PATCH"
        };
        
        public Request(Stream network)
        {
            IsValid = false;
            Url = new Url(null);
            Headers = new Dictionary<string, string>();
            HeaderCount = 0;
            
            // TODO: Request Body
            ContentStream = null;
            ContentString = null;
            ContentBytes = null;
            
            // network stream is not empty
            if ((int)network.Length > 1)
            {
                // convert data from network stream to string
                StreamReader reader = new StreamReader(network);
                string networkString = reader.ReadToEnd();
                ContentLength = networkString.Length;
                string[] networkLines = networkString.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                // get first line of network stream and check if its valid
                string[] reqLine = networkLines.First().Split(" ");
                if (reqLine.Length == 3)
                {
                    Url = new Url(reqLine[1]);
                    Method = reqLine[0].ToUpper();

                    // method is valid
                    if (_validRequestMethods.Contains(Method))
                    {
                        // get headers form network stream data
                        foreach (var line in networkLines.Skip(1))
                        {
                            HeaderCount++;
                            string key = line.Split(":").First().ToLower();
                            string value = line.Split(":").Last().Trim();
                            Headers[key] = value;
                        }

                        if (Headers.ContainsKey("user-agent"))
                        {
                            UserAgent = Headers["user-agent"];
                        }
                    
                        if (Headers.ContainsKey("content-type"))
                        {
                            ContentType = Headers["content-type"];
                        }
                        
                        IsValid = true;
                    }
                }
            }
            
            Console.WriteLine("-------------------------------");
            Console.WriteLine("IsValid: " + IsValid);
            Console.Write("Headers:\n");
            foreach (var head in Headers)
            {
                Console.WriteLine(head.Key + ": " + head.Value);
            }
            Console.WriteLine("HeaderCount: " + HeaderCount);
            Console.WriteLine("Method: " + Method);
            Console.WriteLine("User-Agent: " + UserAgent);
            Console.WriteLine("Content-Type: " + ContentType);
            Console.WriteLine("-------------------------------");
        }
    }
}