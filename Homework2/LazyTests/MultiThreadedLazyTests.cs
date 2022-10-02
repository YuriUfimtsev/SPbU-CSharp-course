namespace LazyTests;

using Lazy;

public class MultiThreadedLazyTests
{
    private static int testIterationsNumber = 1000;
    private static ManualResetEvent testIterationStarted = new ManualResetEvent(false);

    [Test]
    public void MultiThreadedTest()
    {
        var random = new Random();
        var threads = new Thread[Environment.ProcessorCount];
        var threadsCalculationResult = new int[threads.Length];
        for (var k = 0; k < testIterationsNumber; ++k)
        {
            testIterationStarted.Reset();
            var lazyObject = new MultiThreadedLazy<int>(() => random.Next(1000));

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
