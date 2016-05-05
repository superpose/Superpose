using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Http;

namespace SuperposeLib.Owin
{
    public class ValuesController : ApiController
    {
        /*         
                 $scope.downloadFile = function(downloadPath) { 
                   window.open(downloadPath, '_blank', '');  
                  }

                 var downloadPath = "/files/instructions.pdf";
                 $scope.downloadFile(downloadPath);
             */

        public HttpResponseMessage GetFile(string name)
        {
            var path = name;
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

        //// GET api/values 
        public IEnumerable<string> Get()
        {
            return new[] {"value1", "value2"};
        }

        [Authorize]
        public IEnumerable<string> Protected()
        {
            /*
            if we receive a JWT, we’ll validate it and if it checks out we’ll pass the corresponding ClaimsPrincipal to the API body
            The UPN claim is among the ones that Windows Azure AD sends in JWTs 

             */
            Console.WriteLine("==>I have been called by {0}", ClaimsPrincipal.Current.FindFirst(ClaimTypes.Upn));
            return new[] {"value1", "value2"};
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}