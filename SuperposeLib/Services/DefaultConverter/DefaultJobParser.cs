using Newtonsoft.Json;
using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Interfaces.JobThings;
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