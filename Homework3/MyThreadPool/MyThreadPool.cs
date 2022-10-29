namespace MyThreadPool;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MyThreadPool
{
    private int threadCount;
    private ConcurrentQueue<Action> tasksQueue;
    private MyThread[] myThreadArray;
    private CancellationTokenSource cancellationTokenSource;

    public MyThreadPool(int threadCount)
    {
        if (threadCount <= 0)
        {
            throw new InvalidDataException();
        }

        this.threadCount = threadCount;
        this.tasksQueue = new ConcurrentQueue<Action>();
        this.myThreadArray = new MyThread[threadCount];
        this.cancellationTokenSource = new CancellationTokenSource();
        for (var i = 0; i < threadCount; ++i)
        {
            this.myThreadArray[i] = new MyThread(this, this.cancellationTokenSource.Token);
        }
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> function, ManualResetEvent? parentalTaskResetEvent)
    {
        var myTask = parentalTaskResetEvent == null ? new MyTask<TResult>(function, this)
            : new MyTask<TResult>(function, this, parentalTaskResetEvent);
        this.tasksQueue.Enqueue(() => myTask.Compute());
        return myTask;
    }

    public void ShutDown()
    {
        this.cancellationTokenSource.Cancel();
        for (var i = 0; i < this.myThreadArray.Length; ++i)
        {
            this.myThreadArray[i].ThreadJoin();
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
            while (true)
            {
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

        public void ThreadJoin()
        {
            this.realThread.Join();
        }
    }
}
