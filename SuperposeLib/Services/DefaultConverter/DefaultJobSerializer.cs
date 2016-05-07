using Newtonsoft.Json;
using Superpose.StorageInterface;
using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Services.DefaultConverter
{
    public class DefaultJobSerializer : IJobSerializer
    {
        public string Execute<TJob>(TJob jobLoad) where TJob : IJobLoad
        {
            return JsonConvert.SerializeObject(jobLoad);
        }
    }
}