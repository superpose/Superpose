using Newtonsoft.Json;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Services.DefaultConverter
{
    public class DefaultJobSerializer : IJobSerializer
    {
        public string Execute(object jobLoad) 
        {
            return JsonConvert.SerializeObject(jobLoad);
        }
    }
}