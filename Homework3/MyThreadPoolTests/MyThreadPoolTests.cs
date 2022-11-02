using MyThreadPool;

namespace MyThreadPoolTests;

public class MyThreadPoolTests
{
    private int threadsInMyThreadPoolCount = 8;
    private MyThreadPool.MyThreadPool myThreadPool;
    private ManualResetEvent manualResetEvent;

    [SetUp]
    public void Initialize()
    {
        this.myThreadPool = new MyThreadPool.MyThreadPool(this.threadsInMyThreadPoolCount);
        this.manualResetEvent = new ManualResetEvent(false);
    }

    [Test]
    public void CheckActualNumberOfMyThreadPoolThreadsTest()
    {
        for (var i = 0; i < this.threadsInMyThreadPoolCount; ++i)
        {
            this.myThreadPool.Submit(() =>
            {
                this.manualResetEvent.WaitOne();
                return 0;
            });
        }

        Thread.Sleep(60);
        Assert.That(this.myThreadPool.AreAllTasksInProgress);
        this.myThreadPool.Submit(() =>
        {
            this.manualResetEvent.WaitOne();
            return 0;
        });
        Thread.Yield();
        Assert.That(!this.myThreadPool.AreAllTasksInProgress);
        this.manualResetEvent.Set();
        this.myThreadPool.ShutDown();
    }

    [Test]
    public void PerformManyTasksTest()
    {
        var taskArray = new IMyTask<int>[this.threadsInMyThreadPoolCount * 3];
        for (var i = 0; i < this.threadsInMyThreadPoolCount * 3; ++i)
        {
            taskArray[i] = this.myThreadPool.Submit(() => CalculateArithmeticProgression(1000));
        }

        var expectedResultArray = new int[taskArray.Length];
        var resultArray = new int[taskArray.Length];
        for (var j = 0; j < taskArray.Length; ++j)
        {
            resultArray[j] = taskArray[j].Result;
            expectedResultArray[j] = 499500;
        }

        Assert.That(resultArray, Is.EqualTo(expectedResultArray));
        this.myThreadPool.ShutDown();
    }

    [Test]
    public void PerformManyTasksWithFullLoadThreadsTest()
    {
        var taskArray = new IMyTask<int>[this.threadsInMyThreadPoolCount * 4];
        for (var i = 0; i < this.threadsInMyThreadPoolCount * 4; ++i)
        {
            taskArray[i] = this.myThreadPool.Submit(() =>
            {
                var result = 0;
                for (var j = 0; j < 1000; ++j)
                {
                    result += j;
                }

                this.manualResetEvent.WaitOne();
                return result;
            });
        }

        this.manualResetEvent.Set();
        var expectedResultArray = new int[taskArray.Length];
        var resultArray = new int[taskArray.Length];
        for (var j = 0; j < taskArray.Length; ++j)
        {
            resultArray[j] = taskArray[j].Result;
            expectedResultArray[j] = 499500;
        }

        Assert.That(resultArray, Is.EqualTo(expectedResultArray));
        this.myThreadPool.ShutDown();
    }

    [Test]
    public void PerformSmallNumberOfTasksWithIntervals()
    {
        var taskArray = new IMyTask<string>[this.threadsInMyThreadPoolCount];
        for (var i = 0; i < this.threadsInMyThreadPoolCount; ++i)
        {
            var locali = i;
            taskArray[i] = this.myThreadPool.Submit(() =>
            {
                return locali.ToString();
            });
        }

        var expectedResultArray = new string[taskArray.Length];
        var resultArray = new string[taskArray.Length];
        for (var j = 0; j < taskArray.Length; ++j)
        {
            resultArray[j] = taskArray[j].Result;
            expectedResultArray[j] = j.ToString();
        }

        Assert.That(resultArray, Is.EqualTo(expectedResultArray));
        this.myThreadPool.ShutDown();
    }

    [Test]
    public void CorrectShutdownWorkTest()
    {
        var taskArray = new IMyTask<string>[this.threadsInMyThreadPoolCount+ 2];
        for (var i = 0; i < this.threadsInMyThreadPoolCount + 2; ++i)
        {
            var locali = i;
            taskArray[i] = this.myThreadPool.Submit(() =>
            {
                this.manualResetEvent.WaitOne();
                Thread.Sleep(200);
                return locali.ToString();
            });
        }

        Thread.Sleep(60);
        this.manualResetEvent.Set();
        this.myThreadPool.ShutDown();
        for (var j = 0; j < this.threadsInMyThreadPoolCount + 2; ++j)
        {
            if (j < this.threadsInMyThreadPoolCount)
            {
                Assert.That(taskArray[j].Result, Is.EqualTo(j.ToString()));
            }
            else
            {
                Assert.Throws<ShutdownRequestedException>(() => { var result = taskArray[j].Result; });
            }
        }
    }

    [Test]
    public void ShutdownWithInfiniteFunctionsTest()
    {
        var taskArray = new IMyTask<int>[this.threadsInMyThreadPoolCount];
        for (var i = 0; i < this.threadsInMyThreadPoolCount; ++i)
        {
            var locali = i;
            taskArray[i] = this.myThreadPool.Submit(() =>
            {
                return DoSomethingEndlessly();
            });
        }

        Assert.Throws<EndlessFunctionException>(() => this.myThreadPool.ShutDown());
    }

    [Test]
    public void IsCompletedPropertyTest()
    {
        var newTask = this.myThreadPool.Submit(() =>
        {
            this.manualResetEvent.WaitOne();
            return CalculateArithmeticProgression(100);
        });

        Assert.That(newTask.IsCompleted, Is.False);
        this.manualResetEvent.Set();
        var result = newTask.Result;
        Assert.That(newTask.IsCompleted, Is.True);
        Assert.That(result, Is.EqualTo(4950));
    }

    [Test]
    public void ResultPropertyThrowsExceptionTest()
    {
        var newTask = this.myThreadPool.Submit(() =>
        {
            return ThrowException();
        });

        try
        {
            var result = newTask.Result;
        }
        catch (Exception exception)
        {
            Assert.That(exception, Is.TypeOf<AggregateException>());
            Assert.That(exception.InnerException, Is.TypeOf<InvalidDataException>());
        }
    }

    [Test]
    public void ContinuationPropertyIsCompletedAndResultReadinessAfterBasicTaskResultReadinessTest()
    {
        var myTask = myThreadPool.Submit(() => CalculateArithmeticProgression(100));
        var continuationTask = myTask.ContinueWith(x => x.ToString());
        var continuationResult = continuationTask.Result;
        Assert.That(myTask.IsCompleted, Is.True);
        Assert.That(continuationTask.IsCompleted, Is.True);
        Assert.That(continuationResult, Is.EqualTo("4950"));
    }

    [Test]
    public void PerformManyContinuationTasksWithFullLoadThreadsTest()
    {
        var taskArray = new IMyTask<int>[this.threadsInMyThreadPoolCount * 4];
        var myTask = myThreadPool.Submit(() => CalculateArithmeticProgression(1000));
        Thread.Sleep(60); // Time to complete the myTask calculation.
        for (var i = 0; i < this.threadsInMyThreadPoolCount * 4; ++i)
        {
            taskArray[i] = myTask.ContinueWith((x) =>
            {
                this.manualResetEvent.WaitOne();
                return x - 1;
            });

            if (i == this.threadsInMyThreadPoolCount - 1)
            {
                Thread.Sleep(60);
                Assert.That(myThreadPool.AreAllTasksInProgress, Is.True);
            }
        }

        this.manualResetEvent.Set();
        var expectedResultArray = new int[taskArray.Length];
        var resultArray = new int[taskArray.Length];
        for (var j = 0; j < taskArray.Length; ++j)
        {
            resultArray[j] = taskArray[j].Result;
            expectedResultArray[j] = 499499;
        }

        Assert.That(resultArray, Is.EqualTo(expectedResultArray));
        this.myThreadPool.ShutDown();
    }

    [Test]
    public void MultipleContinuationCallTest()
    {
        var myComplexTask = myThreadPool.Submit(() => CalculateArithmeticProgression(100))
            .ContinueWith(x => x.ToString())
            .ContinueWith(x => Convert.ToInt32(x))
            .ContinueWith(x => x - 1);
        Assert.That(myComplexTask.Result, Is.EqualTo(4949));
    }

    [Test]
    public void ContinuationWithLastingBasicTask()
    {
        var task = this.myThreadPool.Submit(() =>
        {
            this.manualResetEvent.WaitOne();
            return 0;
        });
        
        var continuationTask = task.ContinueWith(x => x - 1);
        this.manualResetEvent.Set();
        Assert.That(continuationTask.Result, Is.EqualTo(-1));
    }

    [Test]
    public void CorrectShutdownWorkWithContinuationTest()
    {
        var myTask = myThreadPool.Submit(() => CalculateArithmeticProgression(1000));
        Thread.Sleep(60);
        var taskArray = new IMyTask<int>[this.threadsInMyThreadPoolCount + 2];
        for (var i = 0; i < this.threadsInMyThreadPoolCount + 2; ++i)
        {
            var locali = i;
            taskArray[i] = myTask.ContinueWith((x) =>
            {
                this.manualResetEvent.WaitOne();
                Thread.Sleep(200);
                var result = x + locali;
                return result;
            });
        }

        Thread.Sleep(60);
        this.manualResetEvent.Set();
        this.myThreadPool.ShutDown();
        for (var j = 0; j < this.threadsInMyThreadPoolCount + 2; ++j)
        {
            if (j < this.threadsInMyThreadPoolCount)
            {
                Assert.That(taskArray[j].Result, Is.EqualTo(j + 499500));
            }
            else
            {
                Assert.Throws<ShutdownRequestedException>(() => { var result = taskArray[j].Result; });
            }
        }
    }

    private static int CalculateArithmeticProgression(int lastMember)
    {
        var result = 0;
        for (var j = 0; j < lastMember; ++j)
        {
            result += j;
        }

        return result;
    }

    private static int DoSomethingEndlessly()
    {
        var number = 0;
        while (number == 0)
        {
            number = 0;
        }

        return number;
    }

    private static int ThrowException()
    {
        var number = 0;
        if (number == 0)
        {
            throw new InvalidDataException();
        }

        return number;
    }
}