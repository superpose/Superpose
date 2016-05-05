using SuperposeLib.Interfaces.Storage;
using System;

namespace Superpose.Storage.LiteDB
{
    public class LiteDBJobStorage : IJobStorage
    {
        public LiteDBJobStorage(IJobSaver jobSaver, IJobLoader jobLoader, IJobStorageReseter jobStorageReseter)
        {
            if (jobSaver == null) throw new ArgumentNullException(nameof(jobSaver));
            if (jobLoader == null) throw new ArgumentNullException(nameof(jobLoader));
            JobSaver = jobSaver;
            JobLoader = jobLoader;
            JobStorageReseter = jobStorageReseter;
        }

        public IJobSaver JobSaver { get; set; }
        public IJobLoader JobLoader { get; set; }
        public IJobStorageReseter JobStorageReseter { get;  set; }

        public void Dispose()
        {
            JobLoader.Dispose();
            JobSaver.Dispose();
        }
    }
}