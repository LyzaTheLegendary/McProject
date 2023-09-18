using Common.Console;
using Common.Helpers;
using System.Collections.Concurrent;

namespace Common.Threading
{
    public class TaskPool
    {
        private BlockingCollection<Action> taskQueue = new BlockingCollection<Action>();
        private List<CancellationTokenSource> cancelationTokens = new List<CancellationTokenSource>();
        public bool isAlive { get { return !(taskQueue == null); } }
        public TaskPool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                CancellationTokenSource token = new CancellationTokenSource();
                cancelationTokens.Add(token);
                Task.Factory.StartNew(() => { TaskLoop(token.Token); }, TaskCreationOptions.LongRunning);
            }
        }
        public void Stop()
        {
            foreach (CancellationTokenSource token in cancelationTokens)
                token.Cancel();
            //taskQueue.Dispose();
        }
        public void EnqueueTask(Action task)
        {
            if (taskQueue == null)
                return;
            taskQueue.Add(task);
        }
        private void TaskLoop(CancellationToken cancelToken)
        {
            foreach (Action task in taskQueue.GetConsumingEnumerable())
            {
                try
                {
                    task?.Invoke(); // sometimes disposed?
                }catch(Exception ex)
                {
                    Display.WriteError(ErrorHelper.GetErrorMsg(ex));
                }
                if (cancelToken.IsCancellationRequested)
                    return;
            }
        }
    }
}
