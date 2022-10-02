namespace Lazy;

/// <summary>
/// Implements lazy calculations in one thread.
/// </summary>
/// <typeparam name="T"> The type of the return value of the lazy object function.</typeparam>
public class SingleThreadedLazy<T> : ILazy<T>
{
    private readonly Func<T> supplier;
    private T? result;
    private int supplier1;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleThreadedLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier"> Lazy object function.</param>
    public SingleThreadedLazy(Func<T> supplier)
    {
        this.supplier = supplier;
    }

    /// <summary>
    /// Сalculates the result only on the first call. Then returns it.
    /// </summary>
    /// <returns> Result of the lazy object function.</returns>
    public T Get()
    {
        if (this.result == null)
        {
            this.result = this.supplier();
        }

        return this.result;
    }
}
