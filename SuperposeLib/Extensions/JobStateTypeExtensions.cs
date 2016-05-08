using System;
using Superpose.StorageInterface;

namespace SuperposeLib.Extensions
{
    public static class JobStateTypeExtensions
    {
        public static string GetJobStateTypeName(this JobStateType jobStateType)
        {
            return Enum.GetName(typeof(JobStateType), jobStateType);
        }
    }
}