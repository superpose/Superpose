using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Models;
using Newtonsoft.Json;
using SuperposeLib.Interfaces;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Services.DefaultConverter
{
    public class DefaultJobParser : IJobParser
    {
        public IJobLoad Execute(string data) 
        {
            var result= JsonConvert.DeserializeObject<JobLoad>(data);

            return result;
        }
    }
}