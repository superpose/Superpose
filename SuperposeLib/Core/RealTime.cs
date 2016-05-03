using System;
using SuperposeLib.Interfaces;

namespace SuperposeLib.Core
{
    public class RealTime:ITime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}