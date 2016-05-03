using SuperposeLib.Interfaces.Converters;
using SuperposeLib.Models;
using System;

namespace SuperposeLib.Services.DefaultConverter
{
    public class DefaultJobConverter : IJobConverter
    {
        public DefaultJobConverter(IJobParser jobParser, IJobSerializer jobSerializer)
        {
            if (jobParser == null) throw new ArgumentNullException(nameof(jobParser));
            if (jobSerializer == null) throw new ArgumentNullException(nameof(jobSerializer));
            JobParser = jobParser;
            JobSerializer = jobSerializer;
        }

        public IJobParser JobParser { get; set; }
        public IJobSerializer JobSerializer { get; set; }

        public string Serialize<TJob>(TJob jobLoad) where TJob : JobLoad, new()
        {
            return JobSerializer.Execute(jobLoad);
        }

        public TJob Parse<TJob>(string data) where TJob : JobLoad, new()
        {
            return JobParser.Execute<TJob>(data);
        }
    }
}