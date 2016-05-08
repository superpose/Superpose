using Newtonsoft.Json;
using Superpose.StorageInterface.Converters;

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