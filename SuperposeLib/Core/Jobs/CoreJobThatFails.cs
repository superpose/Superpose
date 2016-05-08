using System;
using System.Linq.Expressions;
using Serialize.Linq.Serializers;
using Superpose.StorageInterface;

namespace SuperposeLib.Core.Jobs
{
    public class CoreJobThatFails : AJob
    {
        public CoreJobThatFails(Exception exception, string jobTypeFullName)
        {
            Exception = exception;
            JobTypeFullName = jobTypeFullName;
        }

        private Exception Exception { get; }
        private string JobTypeFullName { get; }

        public override SuperVisionDecision Supervision(Exception reaon, int totalNumberOfHistoricFailures)
        {
            return SuperVisionDecision.Fail;
        }

        protected override void Execute()
        {
            throw new Exception("Unable to run job " + JobTypeFullName, Exception);
        }
    }


    public class LinqJob : AJob<LinqJobCommand>
    {
        protected override void Execute(LinqJobCommand command = null)
        {
            var serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());
            
            if (command != null)
            {
                var result = serializer.DeserializeText(command.ExpressionString) as Expression<Action>;

                if (result == null)
                {
                  var  result2 = serializer.DeserializeText(command.ExpressionString) as Expression<Action<dynamic>>;
                    result2?.Compile().Invoke(command.Context);
                }
                else
                {
                    var c = result.Compile();
                   c .Invoke(); 
                }
            }
            else
            {
                throw new Exception("cannot find expression");
            }
        }
    }

    public class LinqJobCommand : AJobCommand
    {
        public  dynamic Context { set; get; }
        public  string ExpressionString { set; get; }
    }
}