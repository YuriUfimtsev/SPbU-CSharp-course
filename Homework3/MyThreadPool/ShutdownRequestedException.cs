namespace MyThreadPool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Implements exception that is thrown if the ThreadPool's task queue wasn't empty when Shutdown was requested.
/// </summary>
public class ShutdownRequestedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShutdownRequestedException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    public ShutdownRequestedException(string message)
        : base(message)
    {
    }
}
