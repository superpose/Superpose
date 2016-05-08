using Newtonsoft.Json;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;
using SuperposeLib.Models;

namespace SuperposeLib.Services.DefaultConverter
{
    public class DefaultJobParser : IJobParser
    {
        public IJobLoad Execute(string data)
        {
            var result = JsonConvert.DeserializeObject<JobLoad>(data);

            return result;
        }
    }
}