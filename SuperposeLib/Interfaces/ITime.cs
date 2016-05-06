using System;

namespace SuperposeLib.Interfaces
{
    public interface ITime
    {
        DateTime UtcNow { get; }

        DateTime MinValue { get; }
    }
}