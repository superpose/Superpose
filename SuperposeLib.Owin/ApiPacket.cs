using System;
using Microsoft.Owin;

namespace SuperposeLib.Owin
{
    public class ApiPacket
    {
        public ApiPacket()
        {
            TrackingId = Guid.NewGuid().ToString();
        }

        public string CallerIdentity { get; set; }
        public string Response { get; set; }
        public string TrackingId { get; set; }
        public string Verb { get; set; }
        public Uri RequestUri { get; set; }
        public IHeaderDictionary RequestHeaders { get; set; }
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public IHeaderDictionary ResponseHeaders { get; set; }
        public string Request { get; set; }
    }
}