using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    /// <summary>
    /// Url class that processes an URL string.
    /// </summary>
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

        /// <summary>
        /// Process URL path string.
        /// </summary>
        /// <param name="path"></param>
        public Url(string path)
        {
            _path = path;
            ParameterCount = 0;

            // process url parameters
            string[] splitUrlQuestionMark = RawUrl.Split('?');

            // url contains one ? and is not empty or null before and after ?
            if (splitUrlQuestionMark.Length == 2 && !String.IsNullOrEmpty(splitUrlQuestionMark[0]) &&
                !String.IsNullOrEmpty(splitUrlQuestionMark[1]))
            {
                // split at & and remove fragments
                string[] splitUrlParams = splitUrlQuestionMark[1].Split("&");
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