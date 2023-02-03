namespace Lazy;

/// <summary>
/// Implements lazy calculations.
/// </summary>
/// <typeparam name="T"> The type of the return value of the lazy object function.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Сalculates the result only on the first call. Then returns it.
    /// </summary>
    /// <returns> Result of the lazy object function.</returns>
    T? Get();
}