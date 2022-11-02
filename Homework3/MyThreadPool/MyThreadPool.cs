namespace MyThreadPool;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

/// <summary>
/// Implements the ThreadPool abstraction.
/// </summary>
public class MyThreadPool
{
    private int millisecondsTimeoutForTaskInJoiningThread;
    private ConcurrentQueue<Action> tasksQueue;
    private MyThread[] myThreadArray;
    private CancellationTokenSource cancellationTokenSource;
    private AutoResetEvent autoResetEvent;
    private ManualResetEvent manualResetEvent;
    private WaitHandle[] resetEventsArray;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadCount">Number of this ThreadPool threads.</param>
    /// <param name="maxDelayTimeForThreadPoolShutdown">Time in milliseconds that you are ready waiting for Shutdown method.</param>
    /// <exception cref="InvalidDataException">Throws if the number of this ThreadPool threads less than or equal to zero.</exception>
    public MyThreadPool(
        int threadCount,
        int maxDelayTimeForThreadPoolShutdown = 30000)
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

        this.millisecondsTimeoutForTaskInJoiningThread = maxDelayTimeForThreadPoolShutdown / threadCount;
    }

    /// <summary>
    /// Gets the number of this ThreadPool threads.
    /// </summary>
    public int ThreadCount { get; }

    /// <summary>
    /// Gets a value indicating whether this Threadpool's Shutdown was requested.
    /// </summary>
    public bool IsShutdownRequested { get; private set; }

    /// <summary>
    /// Gets a value indicating whether all tasks given to the ThreadPool are being computed.
    /// </summary>
    public bool AreAllTasksInProgress
    {
        get
        {
            return this.tasksQueue.Count == 0;
        }
    }

    /// <summary>
    /// Submits new task to the ThreadPool.
    /// </summary>
    /// <typeparam name="TResult">Return value type of the task function.</typeparam>
    /// <param name="function">Task function.</param>
    /// <param name="parentalTaskResetEvent">Parental task ManualResetEvent (for continuation task).</param>
    /// <returns>Task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function, ManualResetEvent? parentalTaskResetEvent = null)
    {
        var myTask = parentalTaskResetEvent == null ? new MyTask<TResult>(function, this)
            : new MyTask<TResult>(function, this, parentalTaskResetEvent);
        this.tasksQueue.Enqueue(() => myTask.Compute());
        this.autoResetEvent.Set();
        return myTask;
    }

    /// <summary>
    /// Shuts down the ThreadPool's work.
    /// </summary>
    /// <exception cref="EndlessFunctionException">Throws if the function hasn't been finished for definite time.</exception>
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

                if (this.myThreadPool.tasksQueue.Count > 0)
                {
                    this.myThreadPool.manualResetEvent.Set();
                }
                else
                {
                    this.myThreadPool.manualResetEvent.Reset();
                }
            }
        }

        public void ThreadJoin(int millisecondsTimeoutForTaskInJoiningThread)
        {
            this.realThread.Join(millisecondsTimeoutForTaskInJoiningThread);
        }
    }
}
