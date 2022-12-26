namespace MyNUnit;

using System.Reflection;
using MyNUnit.Information;
using MyNUnit.Attributes;
using System.Diagnostics;

/// <summary>
/// Implements the test method entitie.
/// </summary>
public class Test
{
    private readonly Type? expectedException;
    private readonly object classObject;
    private readonly Stopwatch stopwatch;
    private readonly string reasonForIgnoring;
    private readonly bool isIgnored;
    private readonly MethodInfo methodInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="Test"/> class.
    /// </summary>
    /// <param name="methodInfo">Method information.</param>
    /// <param name="classObject">Object of the class to which test method belongs.</param>
    public Test(MethodInfo methodInfo, object classObject)
    {
        this.methodInfo = methodInfo;
        var testAttribute = this.methodInfo.GetCustomAttribute(typeof(TestAttribute));
        this.isIgnored = ((TestAttribute)testAttribute!).Ignore != null;
        this.reasonForIgnoring = this.isIgnored ? ((TestAttribute)testAttribute!).Ignore! : string.Empty;
        this.expectedException = ((TestAttribute)testAttribute!).Expected;
        this.classObject = classObject;
        this.stopwatch = new ();
    }

    /// <summary>
    /// Runs the test.
    /// </summary>
    /// <returns>Information about running the test.</returns>
    public TestInfo Run()
    {
        this.stopwatch.Reset();
        if (this.isIgnored)
        {
            return new TestInfo(this.methodInfo.Name, TestStatus.Status.Ignored, $"Reason for ignoring: {this.reasonForIgnoring}");
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
            var catchedExceptionType = exception.InnerException!.GetType();
            if (this.expectedException != null && this.expectedException == catchedExceptionType)
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
