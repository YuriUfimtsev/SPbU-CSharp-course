namespace MyThreadPool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Interface of a task abstraction.
/// </summary>
/// <typeparam name="TResult">Return value type of task function.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether the task result is ready.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets the task result.
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Creates a new task based on this task.
    /// </summary>
    /// <typeparam name="TNewResult">Return value type for the new task function.</typeparam>
    /// <param name="func">Intermediate function for creating a new task.</param>
    /// <returns>Task with new return value type of the function.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
}
