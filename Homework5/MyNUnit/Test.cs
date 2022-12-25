namespace MyNUnit;

using System.Reflection;
using MyNUnit.Information;
using MyNUnit.Attributes;
using System.Diagnostics;

public class Test
{
    private readonly Exception? expectedException;
    private readonly object classObject;
    private readonly Stopwatch stopwatch;
    private readonly string reasonForIgnoring;
    private readonly bool isIgnored;
    private readonly MethodInfo methodInfo;

    public Test(MethodInfo methodInfo, object classObject)
    {
        this.methodInfo = methodInfo;
        var testAttribute = this.methodInfo.GetCustomAttribute(typeof(TestAttribute));
        this.isIgnored = ((TestAttribute)testAttribute!).Ignore != null;
        this.reasonForIgnoring = this.isIgnored ? string.Empty : ((TestAttribute)testAttribute!).Ignore!;
        this.expectedException = ((TestAttribute)testAttribute!).Expected;
        this.classObject = classObject;
        this.stopwatch = new ();
    }

    public TestInfo Run()
    {
        this.stopwatch.Reset();
        if (this.isIgnored)
        {
            return new TestInfo(this.methodInfo.Name, TestStatus.Status.Ignored, this.reasonForIgnoring);
        }

        this.stopwatch.Start();
        try
        {
            this.methodInfo.Invoke(this.classObject, new object[] { });
            this.stopwatch.Stop();
            return new TestInfo(this.methodInfo.Name, this.stopwatch.ElapsedMilliseconds, TestStatus.Status.Passed);
        }
        catch (Exception exception)
        {
            this.stopwatch.Stop();
            var catchedExceptionType = exception.GetType();
            if (this.expectedException != null && this.expectedException.GetType() == catchedExceptionType)
            {
                return new TestInfo(this.methodInfo.Name, this.stopwatch.ElapsedMilliseconds, TestStatus.Status.Passed);
            }
            else
            {
                return new TestInfo(
                    this.methodInfo.Name,
                    this.stopwatch.ElapsedMilliseconds,
                    TestStatus.Status.Failed,
                    $"Catched {catchedExceptionType} exception.");
            }
        }
    }
}
