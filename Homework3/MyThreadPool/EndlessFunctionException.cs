namespace MyThreadPool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EndlessFunctionException : Exception
{
    public EndlessFunctionException(string message)
        : base(message)
    {
    }
}
