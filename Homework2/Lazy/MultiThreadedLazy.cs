namespace Lazy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public class MultiThreadedLazy<T> : ILazy<T>
{
    private readonly Func<T> supplier;
    private object? result;
    private volatile bool isResultCalculate;
    private object synchronizationObject;

    public MultiThreadedLazy(Func<T> supplier)
    {
        this.supplier = supplier;
        this.synchronizationObject = new object();
        this.isResultCalculate = false;
    }

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
