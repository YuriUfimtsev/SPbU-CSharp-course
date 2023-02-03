namespace MyNUnit.Information;

/// <summary>
/// Provides information about running the test method.
/// </summary>
public class TestInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestInfo"/> class.
    /// </summary>
    /// <param name="name">Test name.</param>
    /// <param name="executionResult">Test execution status.</param>
    public TestInfo(string name, TestStatus.Status executionResult)
        : this(name, 0, executionResult, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestInfo"/> class.
    /// </summary>
    /// <param name="name">Test name.</param>
    /// <param name="executionResult">Test execution status.</param>
    /// <param name="report">Additional information about the test execution.</param>
    public TestInfo(string name, TestStatus.Status executionResult, string report)
        : this(name, 0, executionResult, report)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestInfo"/> class.
    /// </summary>
    /// <param name="name">Test name.</param>
    /// <param name="duration">Test execution time in milliseconds.</param>
    /// <param name="executionResult">Test execution status.</param>
    public TestInfo(string name, long duration, TestStatus.Status executionResult)
        : this(name, duration, executionResult, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestInfo"/> class.
    /// </summary>
    /// <param name="name">Test name.</param>
    /// <param name="duration">Test execution time in milliseconds.</param>
    /// <param name="executionResult">Test execution status.</param>
    /// <param name="report">Additional information about the test execution.</param>
    public TestInfo(string name, long duration, TestStatus.Status executionResult, string report)
    {
        this.Name = name;
        this.Duration = duration;
        this.ExecutionResult = executionResult;
        this.Report = report;
    }

    /// <summary>
    /// Gets the test name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the test execution time in milliseconds.
    /// </summary>
    public long Duration { get; }

    /// <summary>
    /// Gets the test execution status.
    /// </summary>
    public TestStatus.Status ExecutionResult { get; }

    /// <summary>
    /// Gets the additional information about the test execution.
    /// </summary>
    public string Report { get; }
}
