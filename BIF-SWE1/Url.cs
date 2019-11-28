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

            // url contains one ? and is not empty or null before and after ?
            if (splitUrlQuestionmark.Length == 2 && !String.IsNullOrEmpty(splitUrlQuestionmark[0]) &&
                !String.IsNullOrEmpty(splitUrlQuestionmark[1]))
            {
                // split at & and remove fragments
                string[] splitUrlParams = splitUrlQuestionmark[1].Split("&");
                splitUrlParams[splitUrlParams.GetUpperBound(0)] = splitUrlParams.Last().Split("#").First();

                foreach (var param in splitUrlParams)
                {
                    string[] urlParams = param.Split("=");
                    if (urlParams.Length != 2) break;
                    ParameterCount++;
                    Parameter[urlParams[0]] = urlParams[1];
                }
            }
        }
    }
}