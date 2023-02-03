namespace MyThreadPool;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    private AutoResetEvent newTaskEvent;
    private ManualResetEvent hasTasksInQueueEvent;
    private WaitHandle[] resetEventsArrayForCorrectWaitingForTasks;
    private object lockObject;

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
        this.newTaskEvent = new AutoResetEvent(false);
        this.hasTasksInQueueEvent = new ManualResetEvent(false);
        this.resetEventsArrayForCorrectWaitingForTasks = new WaitHandle[2]
        {
            this.newTaskEvent,
            this.hasTasksInQueueEvent,
        };
        for (var i = 0; i < threadCount; ++i)
        {
            this.myThreadArray[i] = new MyThread(this, this.cancellationTokenSource.Token);
        }

        this.millisecondsTimeoutForTaskInJoiningThread = maxDelayTimeForThreadPoolShutdown / threadCount;
        this.lockObject = new object();
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
    public bool AreAllTasksInProgress => this.tasksQueue.Count == 0;

    /// <summary>
    /// Submits new task to the ThreadPool.
    /// </summary>
    /// <typeparam name="TResult">Return value type of the task function.</typeparam>
    /// <param name="function">Task function.</param>
    /// <param name="parentalTaskResetEvent">Parental task ManualResetEvent (for continuation task).</param>
    /// <returns>Task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function, ManualResetEvent? parentalTaskResetEvent = null)
    {
        lock (this.lockObject)
        {
            if (this.cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException("Shutdown was previously requested");
            }

            var myTask = parentalTaskResetEvent == null ? new MyTask<TResult>(function, this)
                : new MyTask<TResult>(function, this, parentalTaskResetEvent);
            lock (this.tasksQueue)
            {
                this.tasksQueue.Enqueue(() => myTask.Compute());
            }

            this.newTaskEvent.Set();
            return myTask;
        }
    }

    /// <summary>
    /// Shuts down the ThreadPool's work.
    /// </summary>
    /// <exception cref="EndlessFunctionException">Throws if the function hasn't been finished for definite time.</exception>
    public void ShutDown()
    {
        lock (this.lockObject)
        {
            this.cancellationTokenSource.Cancel();
            this.hasTasksInQueueEvent.Set();
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
                WaitHandle.WaitAny(this.myThreadPool.resetEventsArrayForCorrectWaitingForTasks);
                if (token.IsCancellationRequested)
                {
                    break;
                }

                lock (this.myThreadPool.tasksQueue)
                {
                    if (this.myThreadPool.tasksQueue.Count > 0)
                    {
                        this.myThreadPool.hasTasksInQueueEvent.Set();
                    }
                    else
                    {
                        this.myThreadPool.hasTasksInQueueEvent.Reset();
                    }
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

                lock (this.myThreadPool.tasksQueue)
                {
                    if (this.myThreadPool.tasksQueue.Count > 0)
                    {
                        this.myThreadPool.hasTasksInQueueEvent.Set();
                    }
                    else
                    {
                        this.myThreadPool.hasTasksInQueueEvent.Reset();
                    }
                }
            }
        }

        public void ThreadJoin(int millisecondsTimeoutForTaskInJoiningThread)
        {
            this.realThread.Join(millisecondsTimeoutForTaskInJoiningThread);
        }
    }

    /// <summary>
    /// Implements task abstraction.
    /// </summary>
    /// <typeparam name="TResult">Return value type of task function.</typeparam>
    private class MyTask<TResult> : IMyTask<TResult>
    {
        private bool isContinuation;
        private Func<TResult> function;
        private TResult? result;
        private bool isResultReady;

        private List<Action> continuationTaskActions;

        private ManualResetEvent accessToResultEvent;
        private ManualResetEvent? parentalContinuationResetEvent;
        private ManualResetEvent continuationResetEvent;

        private Exception? caughtException;
        private MyThreadPool myThreadPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
        /// </summary>
        /// <param name="function">Task function.</param>
        /// <param name="myThreadPool">ThreadPool that will execute this task.</param>
        /// <param name="previousTaskContinuationResetEvent">Parental task ManualResetEvent (for continuation task).</param>
        public MyTask(
            Func<TResult> function,
            MyThreadPool myThreadPool,
            ManualResetEvent? previousTaskContinuationResetEvent = null)
        {
            this.function = function;
            this.continuationTaskActions = new List<Action>();
            this.accessToResultEvent = new ManualResetEvent(false);
            this.myThreadPool = myThreadPool;
            if (previousTaskContinuationResetEvent != null)
            {
                this.isContinuation = true;
            }

            this.parentalContinuationResetEvent = previousTaskContinuationResetEvent;
            this.continuationResetEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Gets a value indicating whether the task result is ready.
        /// </summary>
        public bool IsCompleted => this.isResultReady;

        /// <summary>
        /// Gets the task result.
        /// </summary>
        public TResult Result
        {
            get
            {
                if (this.myThreadPool.IsShutdownRequested && !this.isResultReady)
                {
                    throw new ShutdownRequestedException("Function wasn't started when the Shutdown was requested.");
                }

                this.accessToResultEvent.WaitOne();
                if (this.caughtException != null)
                {
                    throw new AggregateException(this.caughtException);
                }

                return this.result!;
            }
        }

        /// <summary>
        /// Creates a new task based on this task.
        /// </summary>
        /// <typeparam name="TNewResult">Return value type for the new task function.</typeparam>
        /// <param name="func">Intermediate function for creating a new task.</param>
        /// <returns>Task with new return value type of the function.</returns>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {
            lock (this.continuationTaskActions)
            {
                if (this.result != null)
                {
                    return this.myThreadPool.Submit(() => func(this.Result), this.continuationResetEvent);
                }

                var continuationTask = new MyTask<TNewResult>(
                    () => func(this.Result),
                    this.myThreadPool,
                    this.continuationResetEvent);
                this.continuationTaskActions.Add(() => continuationTask.Compute());
                return continuationTask;
            }
        }

        /// <summary>
        /// Calculates task result.
        /// </summary>
        public void Compute()
        {
            try
            {
                if (this.isContinuation)
                {
                    this.parentalContinuationResetEvent!.WaitOne();
                }

                this.result = this.function();

                lock (this.continuationTaskActions)
                {
                    if (this.continuationTaskActions.Count > 0)
                    {
                        foreach (var continuationTaskAction in this.continuationTaskActions)
                        {
                            this.myThreadPool.Submit(() => continuationTaskAction, this.parentalContinuationResetEvent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.caughtException = ex;
            }

            this.Notify();
        }

        private void Notify()
        {
            this.isResultReady = true;
            this.accessToResultEvent.Set();
            this.continuationResetEvent.Set();
        }
    }
}
