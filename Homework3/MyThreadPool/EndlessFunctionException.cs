namespace MyThreadPool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Implements exception that is thrown if the ThreadPool's task hasn't been finished for definite time after Shutdown was requested.
/// </summary>
public class EndlessFunctionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EndlessFunctionException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    public EndlessFunctionException(string message)
        : base(message)
    {
    }
}
