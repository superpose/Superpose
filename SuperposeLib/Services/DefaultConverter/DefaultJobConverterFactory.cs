using Superpose.StorageInterface.Converters;

namespace SuperposeLib.Services.DefaultConverter
{
    public class DefaultJobConverterFactory : IJobConverterFactory
    {
        public IJobConverter CretateConverter()
        {
            return new DefaultJobConverter(new DefaultJobParser(), new DefaultJobSerializer());
        }
    }
}