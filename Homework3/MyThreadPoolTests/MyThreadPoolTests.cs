using MyThreadPool;
using System.Threading;

namespace MyThreadPoolTests;

public class MyThreadPoolTests
{
    private int threadsInMyThreadPoolCount = 8;
    private MyThreadPool.MyThreadPool myThreadPool;

    [SetUp]
    public void Initialize()
    {
        this.myThreadPool = new MyThreadPool.MyThreadPool(this.threadsInMyThreadPoolCount);
    }

    [Test]
    public void CheckActualNumberOfMyThreadPoolThreadsTest()
    {
        var manualResetEvent = new ManualResetEvent(false);
        for (var i = 0; i < this.threadsInMyThreadPoolCount; ++i)
        {
            this.myThreadPool.Submit(() =>
            {
                manualResetEvent.WaitOne();
                return 0;
            });
        }

        Thread.Sleep(1000);
        Assert.That(this.myThreadPool.AreAllTasksInProgress);
        this.myThreadPool.Submit(() =>
        {
            manualResetEvent.WaitOne();
            return 0;
        });
        Thread.Sleep(1000);
        Assert.That(!this.myThreadPool.AreAllTasksInProgress);
        manualResetEvent.Set();
        this.myThreadPool.ShutDown();
    }

    [Test]
    public void PerformManyTasksTest()
    {
        var manualResetEvent = new ManualResetEvent(false);
        var taskArray = new IMyTask<int>[this.threadsInMyThreadPoolCount * 2];
        for (var i = 0; i < this.threadsInMyThreadPoolCount * 2; ++i)
        {
            var locali = i;
            taskArray[i] = this.myThreadPool.Submit(() =>
            {
                var result = 0;
                for (var locali = 0; locali < 1000; ++locali)
                {
                    result += locali;
                }

                manualResetEvent.WaitOne();
                return result;
            });
        }

        manualResetEvent.Set();

        var expectedResultArray = new int[taskArray.Length];
        var resultArray = new int[taskArray.Length];
        for (var j = 0; j < taskArray.Length; ++j)
        {
            resultArray[j] = taskArray[j].Result;
            expectedResultArray[j] = 499500;
        }

        Assert.That(resultArray, Is.EqualTo(expectedResultArray));
        myThreadPool.ShutDown();
    }

}