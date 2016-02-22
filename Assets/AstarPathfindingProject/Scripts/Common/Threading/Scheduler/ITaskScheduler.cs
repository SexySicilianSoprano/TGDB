
using Ceto.Common.Threading.Tasks;

namespace Ceto.Common.Threading.Scheduling
{
    /// <summary>
    /// Scheduler interface for tasks.
    /// </summary>
    public interface ITaskScheduler
    {

        /// <summary>
        /// If a task is not running on the main thread or uses a
        /// coroutine it needs to tell the scheduler when it is finished. 
        /// The task will then be cleaned up and have its end function called.
        /// </summary>
		void Finished(IThreadedTask task);

        /// <summary>
        /// If a task running on another thread or in a 
        /// coroutine has thrown a exception use this 
        /// function to throw it to the scheduler which
        /// will then rethrow it from the main thread.
        /// </summary>
        void Throw(System.Exception e);

        /// <summary>
        /// Removes a task from the waiting queue and
        /// hands it to the scheduled queue were it will be run.
        /// </summary>
		void StopWaiting(IThreadedTask task, bool run);

    }

}













