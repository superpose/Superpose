using System;
using Superpose.StorageInterface;
using SuperposeLib.Core;

namespace SuperposeLib.Tests.Jobs
{
    public class TestJob : AJob
    {
        protected override void Execute()
        {
            Console.WriteLine("Executed " + GetType().Name);
        }
    }
}