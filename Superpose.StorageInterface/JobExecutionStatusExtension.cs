using System;
using System.Collections.Generic;
using System.Linq;

namespace Superpose.StorageInterface
{
    public static class JobExecutionStatusExtension
    {
        public static List<JobExecutionStatus> ToListOfJobExecutionStatus(this string str)
        {
            var enumList = str.Split(',').ToList().ConvertAll(x => (JobExecutionStatus) Enum.Parse(typeof (JobExecutionStatus), x,true));
            return enumList;
        }
        public static string ToStringOfJobExecutionStatus(this List<JobExecutionStatus> status)
        {
            var enumList = string.Join(",", status.ConvertAll(x => x.ToStringName()));
            return enumList;
        }
        public static string ToStringName(this JobExecutionStatus status)
        {

            return Enum.GetName(typeof(JobExecutionStatus), status);
        }
    }
}