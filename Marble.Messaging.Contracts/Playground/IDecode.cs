using System.Threading.Tasks;

namespace Marble.Messaging.Contracts.Playground
{
    public interface IDecode<I, O>
    {
        /// This may only be called once
        O Instanciate(I input);
        void Next(I input);
        System.Threading.Tasks.Task IsFinished();
    }
}
