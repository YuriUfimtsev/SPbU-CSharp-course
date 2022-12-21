using System.Threading;

namespace ThreadSafePriorityQueueTests;

public class ThreadSafePriorityQueueTests
{
    private ManualResetEvent manualResetEvent;

    [SetUp]
    public void Initialize()
    {
        this.manualResetEvent = new(false);
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void StandartEnqueueTest()
    {
        var threadSafePriorityQueue = new ThreadSafePriorityQueue.ThreadSafePriorityQueue<int>();
        var threads = new Thread[10];
        for (var i = 0; i < 10; ++i)
        {
            var locali = i;
            for (var j = 0; j < 10; ++j)
            {
                var localj = j;
                threads[j] = new Thread(() =>
                {
                    this.manualResetEvent.WaitOne();
                    threadSafePriorityQueue.Enqueue(localj, localj);
                });
            }
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        this.manualResetEvent.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.That(threadSafePriorityQueue.Size, Is.EqualTo(10));
    }

    [Test]
    public void StandartDequeueTest()
    {
        var threadSafePriorityQueue = new ThreadSafePriorityQueue.ThreadSafePriorityQueue<int>();
        var expectedResult = new int[10];
        for (var i = 0; i < 10; ++i)
        {
            expectedResult[i] = i;
            threadSafePriorityQueue.Enqueue(i, i);
        }

        var threads = new Thread[10];
        var threadsResult = new int[10];
        for (var i = 0; i < 10; ++i)
        {
            var locali = i;
            for (var j = 0; j < 10; ++j)
            {
                var localj = j;
                threads[j] = new Thread(() =>
                {
                    this.manualResetEvent.WaitOne();
                    var result = threadSafePriorityQueue.Dequeue();
                    threadsResult[locali] = result;
                });
            }
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        this.manualResetEvent.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        CollectionAssert.AreEqual(threadsResult, expectedResult);
    }
}