using System;
using SuperposeLib.Core;

namespace SuperposeLib.Models
{
    public class JobResult
    {
        public bool IsSuccessfull { set; get; }
        public Exception Exception { get; set; }
        public Exception SuperVisionException { get; set; }
        public SuperVisionDecision SuperVisionDecision { set; get; }
    }
}