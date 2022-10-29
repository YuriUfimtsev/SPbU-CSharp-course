namespace MyThreadPool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IMyTask<TResult>
{
    public bool IsCompleted { get; }

    public TResult Result { get; }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
}
