using MyThreadPool;

namespace MyThreadPoolTests;

public class MyThreadPoolTests
{
    private int threadsInMyThreadPoolCount = 8;

    [Test]
    public void CheckActualNumberOfMyThreadPoolThreadsTest()
    {
        var myThreadPool = new MyThreadPool.MyThreadPool(this.threadsInMyThreadPoolCount);
        var manualResetEvent = new ManualResetEvent(false);
        for (var i = 0; i < this.threadsInMyThreadPoolCount; ++i)
        {
            myThreadPool.Submit(() =>
            {
                manualResetEvent.WaitOne();
                return 0;
            });
        }

        Thread.Sleep(1000);
        Assert.That(myThreadPool.AreAllTasksInProgress);
        myThreadPool.Submit(() =>
        {
            manualResetEvent.WaitOne();
            return 0;
        });
        Thread.Sleep(1000);
        Assert.That(!myThreadPool.AreAllTasksInProgress);
        manualResetEvent.Set();
        myThreadPool.ShutDown();
    }


}