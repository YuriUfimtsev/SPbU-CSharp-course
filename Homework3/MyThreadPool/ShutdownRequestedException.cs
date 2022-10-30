namespace MyThreadPool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ShutdownRequestedException : Exception
{
    public ShutdownRequestedException(string message)
        : base(message)
    {
    }
}
