namespace LazyTests;

using Lazy;

public class MultiThreadedLazyTests
{
    private static int testIterationsNumber = 1000;
    private static ManualResetEvent testIterationStarted = new (false);

    [Test]
    public void MultiThreadedTest()
    {
        var random = new Random();
        var threads = new Thread[10];
        var threadsCalculationResult = new int[threads.Length];
        var counter = 0;
        for (var k = 0; k < testIterationsNumber; ++k)
        {
            testIterationStarted.Reset();
            var lazyObject = new MultiThreadedLazy<int>(() => Interlocked.Increment(ref counter));

            for (var i = 0; i < threads.Length; ++i)
            {
                var locali = i;
                threads[i] = new Thread(() =>
                {
                    var p = testIterationStarted.WaitOne();
                    threadsCalculationResult[locali] = lazyObject.Get();
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            testIterationStarted.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }

            for (var j = 0; j < threads.Length - 1; ++j)
            {
                Assert.That(threadsCalculationResult[j], Is.EqualTo(threadsCalculationResult[j + 1]));
                threadsCalculationResult[j] = random.Next(1000);
            }
        }
    }
}
