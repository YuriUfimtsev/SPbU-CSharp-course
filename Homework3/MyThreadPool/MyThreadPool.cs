namespace MyThreadPool;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

public class MyThreadPool
{
    private int acceptedActionsNumberForManualResetEvent;
    private int millisecondsTimeoutForTaskInJoiningThread;
    private int timeSpanInActionsWhileAllThreadsWork = 1;
    private int acceptedActionsCount;
    private ConcurrentQueue<Action> tasksQueue;
    private MyThread[] myThreadArray;
    private CancellationTokenSource cancellationTokenSource;
    private AutoResetEvent autoResetEvent;
    private ManualResetEvent manualResetEvent;
    private WaitHandle[] resetEventsArray;

    public MyThreadPool(
        int threadCount,
        int maxDelayTimeForThreadPoolShutdown = 30000,
        int functionsNumberToWakeUpAllThreads = 5)
    {
        if (threadCount <= 0)
        {
            throw new InvalidDataException();
        }

        this.ThreadCount = threadCount;

        this.tasksQueue = new ConcurrentQueue<Action>();
        this.myThreadArray = new MyThread[threadCount];
        this.cancellationTokenSource = new CancellationTokenSource();
        this.autoResetEvent = new AutoResetEvent(false);
        this.manualResetEvent = new ManualResetEvent(false);
        this.resetEventsArray = new WaitHandle[2] { this.autoResetEvent, this.manualResetEvent };
        for (var i = 0; i < threadCount; ++i)
        {
            this.myThreadArray[i] = new MyThread(this, this.cancellationTokenSource.Token);
        }

        this.acceptedActionsNumberForManualResetEvent = functionsNumberToWakeUpAllThreads;
        this.millisecondsTimeoutForTaskInJoiningThread = maxDelayTimeForThreadPoolShutdown / threadCount;
    }

    public int ThreadCount { get; }

    public bool IsShutdownRequested { get; private set; }

    public bool AreAllTasksInProgress
    {
        get
        {
            return this.tasksQueue.Count == 0;
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> function, ManualResetEvent? parentalTaskResetEvent = null)
    {
        var myTask = parentalTaskResetEvent == null ? new MyTask<TResult>(function, this)
            : new MyTask<TResult>(function, this, parentalTaskResetEvent);
        this.tasksQueue.Enqueue(() => myTask.Compute());
        ++this.acceptedActionsCount;
        if (this.acceptedActionsCount % this.acceptedActionsNumberForManualResetEvent == 0)
        {
            this.manualResetEvent.Set();
        }

        if (this.acceptedActionsCount % this.acceptedActionsNumberForManualResetEvent
            == this.timeSpanInActionsWhileAllThreadsWork)
        {
            this.manualResetEvent.Reset();
        }

        this.autoResetEvent.Set();
        return myTask;
    }

    public void ShutDown()
    {
        this.cancellationTokenSource.Cancel();
        this.manualResetEvent.Set();
        for (var i = 0; i < this.myThreadArray.Length; ++i)
        {
            this.myThreadArray[i].ThreadJoin(this.millisecondsTimeoutForTaskInJoiningThread);
            if (!this.myThreadArray[i].IsWaiting)
            {
                throw new EndlessFunctionException("The function hasn't been completed" +
                    $" for {this.millisecondsTimeoutForTaskInJoiningThread} milliseconds");
            }
        }

        this.IsShutdownRequested = true;
    }

    private class MyThread
    {
        private Thread realThread;
        private MyThreadPool myThreadPool;
        private Action? taskForComputing;

        public MyThread(MyThreadPool myThreadPool, CancellationToken token)
        {
            this.IsWaiting = true;
            this.myThreadPool = myThreadPool;
            this.realThread = new Thread(() => this.ThreadStart(token));
            this.realThread.Start();
        }

        public bool IsWaiting { get; set; }

        public void ThreadStart(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                WaitHandle.WaitAny(this.myThreadPool.resetEventsArray);
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (!this.myThreadPool.tasksQueue.IsEmpty)
                {
                    var isSuccessful = this.myThreadPool.tasksQueue.TryDequeue(out this.taskForComputing);
                    if (isSuccessful && this.taskForComputing != null)
                    {
                        this.IsWaiting = false;
                        this.taskForComputing();
                        this.IsWaiting = true;
                        this.taskForComputing = null;
                    }
                }
            }
        }

        public void ThreadJoin(int millisecondsTimeoutForTaskInJoiningThread)
        {
            this.realThread.Join(millisecondsTimeoutForTaskInJoiningThread);
        }
    }
}
