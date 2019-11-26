using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    public class Url : IUrl
    {
        private readonly string _path;

        public string RawUrl => _path ?? "";
        public string Path => RawUrl.Split("?").First().Split("#").First();
        public IDictionary<string, string> Parameter { get; } = new Dictionary<string, string>();
        public int ParameterCount { get; }
        public string[] Segments => Path.Split("/", StringSplitOptions.RemoveEmptyEntries);
        public string FileName => Segments.Length > 0 && Segments.Last().Contains(".") ? Segments.Last() : "";
        public string Extension => FileName != "" ? "." + FileName.Split(".").Last() : "";
        public string Fragment => RawUrl.Contains("#") ? RawUrl.Split("#").Last() : "";

        public Url(string path)
        {
            _path = path;
            ParameterCount = 0;
            
            // process url parameters
            string[] splitUrlQuestionmark = RawUrl.Split('?');
           
            // url contains one ?
            if (splitUrlQuestionmark.Length == 2)
            {
                // split at & and remove fragments
                string[] splitUrlParams = splitUrlQuestionmark[1].Split("&");
                splitUrlParams[splitUrlParams.GetUpperBound(0)] = splitUrlParams.Last().Split("#").First();

                foreach (var param in splitUrlParams)
                {
                    ParameterCount++;
                    string[] urlParams = param.Split("=");
                    Parameter[urlParams[0]] = urlParams[1];
                }
            }
            
            // DebugProperties();
        }

        private void DebugProperties()
        {
            Console.WriteLine("-----------DEBUG-URL-----------");
            Console.WriteLine("RawUrl: " + RawUrl);
            Console.WriteLine("Path: " + Path);
            Console.Write("Parameter: ");
            foreach (var param in Parameter)
            {
                Console.Write(param + " ");
            }

            Console.WriteLine("\nParameterCount: " + ParameterCount);
            Console.Write("Segments: ");
            foreach (var seg in Segments)
            {
                Console.Write(seg + " ");
            }

            Console.WriteLine("\nFilename: " + FileName);
            Console.WriteLine("Extension: " + Extension);
            Console.WriteLine("Fragment: " + Fragment);
            Console.WriteLine("-------------------------------");
        }
    }
}