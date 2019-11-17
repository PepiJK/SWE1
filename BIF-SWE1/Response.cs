using System;
using System.Collections.Generic;
using System.IO;
using BIF.SWE1.Interfaces;

namespace BIF_SWE1
{
    public class Response : IResponse
    {
        public IDictionary<string, string> Headers { get; }
        public int ContentLength { get; }
        public string ContentType { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; }
        public string ServerHeader { get; set; }

        public void SetContent(string content)
        {
        }

        public void SetContent(byte[] content)
        {
        }

        public void SetContent(Stream stream)
        {
        }

        public void Send(Stream network)
        {
        }

        public void AddHeader(string header, string value)
        {
        }

        public Response()
        {
        }
    }
}