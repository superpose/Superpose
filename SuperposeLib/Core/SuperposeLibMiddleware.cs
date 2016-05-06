using System.Collections.Generic;
using System.Threading.Tasks;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Services.DefaultConverter;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace SuperposeLib.Core
{
    public class SuperposeLibServerMiddleware
    {
        public SuperposeLibServerMiddleware(AppFunc next)
        {
            Next = next;

            var converter = new DefaultJobConverterFactory().CretateConverter();
            var storage = StorageFactory.CreateJobStorage();
            Runner = new JobRunner(storage, converter);
            Runner.Run();
        }

        public static IJobStoragefactory StorageFactory { set; get; }


        private AppFunc Next { get; }
        private IJobRunner Runner { get; }


        public async Task Invoke(IDictionary<string, object> environment)
        {
            await Next.Invoke(environment);
        }
    }
}