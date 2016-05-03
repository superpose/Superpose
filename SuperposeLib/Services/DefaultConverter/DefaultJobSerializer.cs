using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Models;
using Newtonsoft.Json;

namespace SuperposeLib.Services.DefaultConverter
{
    public class DefaultJobSerializer : IJobSerializer
    {
        public string Execute<TJob>(TJob jobLoad) where TJob : JobLoad, new()
        {
            return JsonConvert.SerializeObject(jobLoad);
        }
    }
}