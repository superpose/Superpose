namespace SuperposeLib.Core.ActorSystem
{
    public class SlimActor<T, TR> : ASlimActor<T, TR>
    {
        public SlimActor(int workerCount=1) : base(workerCount)
        {
        }
    }
}