namespace SuperposeLib.Core.ActorSystem
{
    public class SlimActor<T, TR> : ASlimActor<T, TR>
    {
        public SlimActor(int workerCount) : base(workerCount)
        {
        }
    }
}