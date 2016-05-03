using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Models;
using Newtonsoft.Json;

namespace SuperposeLib.Services.DefaultConverter
{
    public class DefaultJobParser : IJobParser
    {
        public TJob Execute<TJob>(string data) where TJob : JobLoad, new()
        {
            var result= JsonConvert.DeserializeObject<TJob>(data);

            return result;
        }
    }
}