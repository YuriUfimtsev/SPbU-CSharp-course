using System;

namespace Lazy;

/// <summary>
/// Implements lazy calculations in parallel.
/// </summary>
/// <typeparam name="T"> The type of the return value of the lazy object function.</typeparam>
public class MultiThreadedLazy<T> : ILazy<T>
{
    private readonly Func<T> supplier;
    private object? result;
    private volatile bool isResultCalculate;
    private object synchronizationObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiThreadedLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier"> Lazy object function.</param>
    public MultiThreadedLazy(Func<T> supplier)
    {
        this.supplier = supplier;
        this.synchronizationObject = new object();
        this.isResultCalculate = false;
    }

    /// <summary>
    /// Сalculates the result only on the first call. Then returns it.
    /// </summary>
    /// <returns> Result of the lazy object function.</returns>
    public T? Get()
    {
        if (!this.isResultCalculate)
        {
            lock (this.synchronizationObject)
            {
                if (!this.isResultCalculate)
                {
                    Volatile.Write(ref this.result, this.supplier());
                    this.isResultCalculate = true;
                }
            }
        }

        return (T?)this.result;
    }
}
