using Newtonsoft.Json;
using Superpose.StorageInterface.Converters;

namespace SuperposeLib.Services.DefaultConverter
{
    public class DefaultJobSerializer : IJobSerializer
    {
        public string Execute(object jobLoad)
        {
            var result= JsonConvert.SerializeObject(jobLoad);
            return result;
        }
    }
}