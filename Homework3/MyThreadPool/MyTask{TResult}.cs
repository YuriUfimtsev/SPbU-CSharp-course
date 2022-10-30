namespace MyThreadPool;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

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

    public MyTask(Func<TResult> function, MyThreadPool myThreadPool)
    {
        this.function = function;
        this.accessToResultEvent = new ManualResetEvent(false);
        this.myThreadPool = myThreadPool;
        this.continuationResetEvent = new ManualResetEvent(false);
    }

    public MyTask(
        Func<TResult> function,
        MyThreadPool myThreadPool,
        ManualResetEvent previousTaskContinuationResetEvent)
    {
        this.function = function;
        this.accessToResultEvent = new ManualResetEvent(false);
        this.myThreadPool = myThreadPool;
        this.isContinuation = true;
        this.parentalContinuationResetEvent = previousTaskContinuationResetEvent;
        this.continuationResetEvent = new ManualResetEvent(false);
    }

    public bool IsCompleted => this.isResultReady;

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

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> nextFunction)
    {
        return this.myThreadPool.Submit(() => nextFunction(this.Result), this.continuationResetEvent);
    }

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
