using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    public class Url : IUrl
    {
        public string RawUrl { get; set; }
        public string Path { get; set; }
        public IDictionary<string, string> Parameter { get; set; }
        public int ParameterCount { get; set; }
        public string[] Segments { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string Fragment { get; set; }

        public Url(string path)
        {
            RawUrl = path;
            Parameter = new Dictionary<string, string>();
            ParameterCount = 0;
            Segments = new string[0];

            if(RawUrl == null)
            {
                RawUrl = "/";
            }
            else
            {
                // process path
                Path = RawUrl.Split("?").First().Split("#").First();

                // process segments
                Segments = Path.Split("/", StringSplitOptions.RemoveEmptyEntries);

                // url contains parameters
                string[] splitUrlQuestionmark = RawUrl.Split('?');
                if (splitUrlQuestionmark.Length > 2)
                {
                    throw new ArgumentException("Url path is invalid because it contains more than one ?", nameof(splitUrlQuestionmark));
                }
                if (splitUrlQuestionmark.Length == 2)
                {
                    // split at & and remove # part
                    string[] splitUrlParams = splitUrlQuestionmark[1].Split("&");
                    splitUrlParams[splitUrlParams.GetUpperBound(0)] = splitUrlParams.Last().Split("#").First();
                    
                    foreach (var param in splitUrlParams)
                    {
                        ParameterCount++;
                        string[] urlṔarams = param.Split("=");
                        Parameter[urlṔarams[0]] = urlṔarams[1];
                    }
                }

                // url contains a file
                if (RawUrl.Split("/").Last().Split("?").First().Split("#").First().Contains("."))
                {
                    FileName = RawUrl.Split("/").Last().Split("?").First().Split("#").First();
                    Extension = FileName.Split(".").Last();
                }

                // url contains a fragment
                if (RawUrl.Split('/').Last().Contains("#"))
                {
                    Fragment = RawUrl.Split('/').Last().Split("#").Last();
                }
            }
            
            Console.WriteLine("-------------------------------");
            Console.WriteLine("RawUrl: " + RawUrl);
            Console.WriteLine("Path: " + Path);
            Console.Write("Segments: ");
            foreach (var seg in Segments)
            {
                Console.Write(seg + " ");
            }

            Console.WriteLine("\nParameterCount: " + ParameterCount);
            Console.WriteLine("Filename: " + FileName);
            Console.WriteLine("Extension: " + Extension);
            Console.WriteLine("Fragment: " + Fragment);
            Console.Write("Parameters: ");
            foreach (var param in Parameter)
            {
                Console.Write(param + " ");
            }
            Console.WriteLine("\n-------------------------------");
        }
    }
}