using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazy;

public class SingleThreadedLazy<T> : ILazy<T>
{
    private readonly Func<T> supplier;
    private T? result;
    private int supplier1;

    public SingleThreadedLazy(Func<T> supplier)
    {
        this.supplier = supplier;
    }

    public T Get()
    {
        if (this.result == null)
        {
            this.result = this.supplier();
        }

        return this.result;
    }
}
