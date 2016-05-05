using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;
using SuperposeLib.Services.DefaultConverter;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace SuperposeLib.Core
{



    public class SuperposeLibServerMiddleware
    {
        public static IJobStoragefactory StorageFactory { set; get; }
       
       
        private AppFunc Next { set; get; }
        private IJobRunner Runner { set; get; }
        public SuperposeLibServerMiddleware(AppFunc next)
        {
            
            this.Next = next;
           
            var converter = new DefaultJobConverterFactory().CretateConverter();
            var storage = StorageFactory.CreateJobStorage();
                 Runner = new JobRunner(storage, converter);
                Runner.Run();
            
        }



        public async Task Invoke(IDictionary<string, object> environment)
        {
           
            await Next.Invoke(environment);
        }
      

    }
}