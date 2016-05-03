using SuperposeLib.Interfaces.Storage;
using System;

namespace SuperposeLib.Services.InMemoryStorage
{
    public class InMemoryJobStorage : IJobStorage
    {
        public InMemoryJobStorage(IJobSaver jobSaver, IJobLoader jobLoader)
        {
            if (jobSaver == null) throw new ArgumentNullException(nameof(jobSaver));
            if (jobLoader == null) throw new ArgumentNullException(nameof(jobLoader));
            JobSaver = jobSaver;
            JobLoader = jobLoader;
        }

        public IJobSaver JobSaver { get; set; }
        public IJobLoader JobLoader { get; set; }

        public void Dispose()
        {
            JobLoader.Dispose();
            JobSaver.Dispose();
        }
    }
}