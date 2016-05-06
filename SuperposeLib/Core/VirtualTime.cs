using System;
using SuperposeLib.Interfaces;

namespace SuperposeLib.Core
{
    public class VirtualTime : ITime
    {
        public VirtualTime(DateTime nowTime, int sizeOfMilliSecond = 1)
        {
            NowTime = nowTime;
            SizeOfMilliSecond = sizeOfMilliSecond;
            InitialTime = DateTime.UtcNow;
        }

        public DateTime NowTime { get; }
        public int SizeOfMilliSecond { get; }
        private DateTime InitialTime { get; }
        public DateTime MinValue => DateTime.UtcNow.AddYears(-100);

        public DateTime UtcNow
        {
            get
            {
                var elaspsMilliSeconds = (DateTime.UtcNow - InitialTime).TotalMilliseconds;
                var virtualElapseTime = elaspsMilliSeconds*SizeOfMilliSecond;
                return NowTime.AddMilliseconds(virtualElapseTime);
            }
        }
    }
}