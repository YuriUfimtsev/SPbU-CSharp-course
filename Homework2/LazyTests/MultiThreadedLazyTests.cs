using Lazy;
using System.Diagnostics;

namespace LazyTests;

public class MultiThreadedLazyTests
{
    [Test]
    public void MultiThreadedTest()
    {
        var random = new Random();
        var lazyObject = new MultiThreadedLazy<DateTime>(() => DateTime.Now);
        var stopWatch = new Stopwatch();

        var threads = new Thread[Environment.ProcessorCount];
        var threadsCalculationResult = new DateTime[threads.Length];
        var threadsStartingCalculationTime = new TimeSpan[Environment.ProcessorCount];
        for (var i = 0; i < threads.Length; ++i)
        {
            var locali = i;
            threads[i] = new Thread(() =>
            {
                threadsStartingCalculationTime[locali] = stopWatch.Elapsed;
                threadsCalculationResult[locali] = lazyObject.Get();
            });
        }

        stopWatch.Start();
        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        stopWatch.Stop();

        var firstThread = Array.IndexOf(
            threadsStartingCalculationTime,
            threadsStartingCalculationTime.Min());
        var result = threadsCalculationResult[0];
        for (var i = 0; i < threads.Length; ++i)
        {
            Assert.That(result, Is.EqualTo(threadsCalculationResult[i]));
        }

        var abs = result.Subtract(threadsStartingCalculationTime[firstThread]);
        for (var i = 0; i < threads.Length; ++i)
        {
            if (i != firstThread)
            {
                Assert.That(abs, Is.GreaterThanOrEqualTo(result.Subtract(threadsStartingCalculationTime[i])));
            }
        }
    }
}
