namespace MyThreadPool;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Implements task abstraction.
/// </summary>
/// <typeparam name="TResult">Return value type of task function.</typeparam>
public class MyTask<TResult> : IMyTask<TResult>
{
    private bool isContinuation;
    private Func<TResult> function;
    private TResult? result;
    private bool isResultReady;

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
        return this.myThreadPool.Submit(() => func(this.Result), this.continuationResetEvent);
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
