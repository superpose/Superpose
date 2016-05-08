using System;
using SuperposeLib.Core;

namespace SuperposeLib.Tests.Jobs
{
    public class TestJob : AJob
    {
        protected override void Execute(object command)
        {
            Console.WriteLine("Executed " + GetType().Name);
        }
    }
}