using System;
using System.Linq.Expressions;
using Serialize.Linq.Serializers;
using Superpose.StorageInterface;

namespace SuperposeLib.Core.Jobs
{
    public class LinqJobCommand : AJobCommand
    {
        public dynamic Context { set; get; }
        public string ExpressionString { set; get; }
    }

    public class LinqJob : AJob<LinqJobCommand>
    {
        protected override void Execute(LinqJobCommand command = null)
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            if (command != null)
            {
                var result = serializer.DeserializeText(command.ExpressionString) as Expression<Action>;

                if (result == null)
                {
                    var result2 = serializer.DeserializeText(command.ExpressionString) as Expression<Action<dynamic>>;
                    result2?.Compile().Invoke(command.Context);
                }
                else
                {
                    result.Compile().Invoke();
                }
            }
            else
            {
                throw new Exception("cannot find expression");
            }
        }
    }
}