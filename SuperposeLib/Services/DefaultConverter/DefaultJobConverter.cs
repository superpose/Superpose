﻿using System;
using Superpose.StorageInterface;
using Superpose.StorageInterface.Converters;

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

        public string SerializeJobLoad(IJobLoad jobLoad)
        {
            return JobSerializer.Execute(jobLoad);
        }

        public string SerializeJobCommand(AJobCommand aJobLoad)
        {
            return JobSerializer.Execute(aJobLoad);
        }

        public IJobLoad Parse(string data)
        {
            return JobParser.Execute(data);
        }
    }
}