using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace SuperposeLib.Owin
{
    public class PacketTrackingMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public PacketTrackingMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);
            var request = context.Request;
            var response = context.Response;

            //capture details about the caller identity

            var identity = (request.User != null && request.User.Identity.IsAuthenticated)
                ? request.User.Identity.Name
                : "(anonymous)";

            var apiPacket = new ApiPacket
            {
                CallerIdentity = identity
            };

            //buffer the request stream in order to intercept downstream reads
            var requestBuffer = new MemoryStream();
            request.Body = requestBuffer;

            //buffer the response stream in order to intercept downstream writes
            var responseStream = response.Body;
            var responseBuffer = new MemoryStream();
            response.Body = responseBuffer;

            //add the "Http-Tracking-Id" response header
            //adding the tracking id response header so that the user
            //of the API can correlate the call back to this entry
            var responseHeaders = (IDictionary<string, string[]>)environment["owin.ResponseHeaders"];
            responseHeaders["http-tracking-id"] = new[] { apiPacket.TrackingId };
            context.Response.OnSendingHeaders(state =>
            {
                var ctx = state as IOwinContext;
                if (ctx == null) return;
                var resp = ctx.Response;



            }, context);

            //invoke the next middleware in the pipeline
            await _next.Invoke(environment);

            //rewind the request and response buffers to record their content
            WriteRequestHeaders(request, apiPacket);
            requestBuffer.Seek(0, SeekOrigin.Begin);
            var requestReader = new StreamReader(requestBuffer);
            apiPacket.Request = await requestReader.ReadToEndAsync();

            WriteResponseHeaders(response, apiPacket);
            responseBuffer.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(responseBuffer);
            apiPacket.Response = await reader.ReadToEndAsync();

            //write the apiPacket to the database
            //await database.InsterRecordAsync(apiPacket);
            System.Diagnostics.Debug.WriteLine("TrackingId: " + apiPacket.TrackingId);

            //make sure the response we buffered is flushed to the client
            responseBuffer.Seek(0, SeekOrigin.Begin);
            await responseBuffer.CopyToAsync(responseStream);
        }
        private static void WriteRequestHeaders(IOwinRequest request, ApiPacket packet)
        {
            packet.Verb = request.Method;
            packet.RequestUri = request.Uri;
            packet.RequestHeaders = request.Headers;
        }
        private static void WriteResponseHeaders(IOwinResponse response, ApiPacket packet)
        {
            packet.StatusCode = response.StatusCode;
            packet.ReasonPhrase = response.ReasonPhrase;
            packet.ResponseHeaders = response.Headers;
        }
    }
}