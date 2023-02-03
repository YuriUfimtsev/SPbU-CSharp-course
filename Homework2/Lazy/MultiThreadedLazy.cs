using System;

namespace Lazy;

/// <summary>
/// Implements lazy calculations in parallel.
/// </summary>
/// <typeparam name="T"> The type of the return value of the lazy object function.</typeparam>
public class MultiThreadedLazy<T> : ILazy<T>
{
    private Func<T?>? supplier;
    private T? result;
    private volatile bool isResultCalculated;
    private object synchronizationObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiThreadedLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier"> Lazy object function.</param>
    public MultiThreadedLazy(Func<T?> supplier)
    {
        this.supplier = supplier;
        this.synchronizationObject = new object();
        this.isResultCalculated = false;
    }

    /// <summary>
    /// Сalculates the result only on the first call. Then returns it.
    /// </summary>
    /// <returns> Result of the lazy object function.</returns>
    public T? Get()
    {
        if (!this.isResultCalculated)
        {
            lock (this.synchronizationObject)
            {
                if (!this.isResultCalculated)
                {
                    this.result = this.supplier!();
                    this.isResultCalculated = true;
                    this.supplier = null;
                }
            }
        }

        return this.result;
    }
}
