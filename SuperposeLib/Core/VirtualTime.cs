using System;
using SuperposeLib.Interfaces;

namespace SuperposeLib.Core
{
    public class VirtualTime : ITime
    {
        public DateTime MinValue => DateTime.UtcNow.AddYears(-100);
        public DateTime NowTime { private set; get; }
        public int SizeOfMilliSecond { private set; get; }
        private DateTime InitialTime { set; get; }
        public VirtualTime(DateTime nowTime, int sizeOfMilliSecond=1)
        {
            NowTime = nowTime;
            SizeOfMilliSecond = sizeOfMilliSecond;
            InitialTime=DateTime.UtcNow;
        }

        public DateTime UtcNow
        {
            get
            {
                var elaspsMilliSeconds = (DateTime.UtcNow - InitialTime).TotalMilliseconds;
                var virtualElapseTime = elaspsMilliSeconds*SizeOfMilliSecond;
                return  NowTime.AddMilliseconds(virtualElapseTime);

            }
          
        }
    }
}