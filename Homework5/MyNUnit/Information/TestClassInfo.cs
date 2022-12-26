namespace MyNUnit.Information;

/// <summary>
/// Provides information about running the test class.
/// </summary>
public class TestClassInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestClassInfo"/> class.
    /// </summary>
    /// <param name="testClassName">Name of the existing test class.</param>
    public TestClassInfo(string testClassName)
    {
        this.TestClassName = testClassName;
        this.TestsInfo = new ();
        this.InvalidTestsNames = new ();
        this.InvalidTestSuitElementsNames = new ();
    }

    /// <summary>
    /// Gets the name of the test class.
    /// </summary>
    public string TestClassName { get; }

    /// <summary>
    /// Gets or sets total execution time of the test suite.
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// Gets information about each test from the test suite.
    /// </summary>
    public List<TestInfo> TestsInfo { get; private set; }

    /// <summary>
    /// Gets names of test methods with the wrong return type or the wrong number of parameters.
    /// </summary>
    public List<string> InvalidTestsNames { get; private set; }

    /// <summary>
    /// Gets the names of the wrong format methods from the test suit.
    /// </summary>
    public List<string> InvalidTestSuitElementsNames { get; private set; }

    /// <summary>
    /// Adds the information about test to the suitable storage.
    /// </summary>
    /// <param name="testInfo">The test information.</param>
    public void AddTest(TestInfo testInfo) => this.TestsInfo.Add(testInfo);

    /// <summary>
    /// Adds the names of the wrong format tests to the suitable storage.
    /// </summary>
    /// <param name="invalidTestsNames">Wrong format tests names.</param>
    public void AddIncorrectTestsNames(List<string> invalidTestsNames) => this.InvalidTestsNames = invalidTestsNames;

    /// <summary>
    /// Adds the names of the wrong format test suit methods to the suitable storage.
    /// </summary>
    /// <param name="invalidTestSuitElementsNames">The names of the wrong format test suit methods.</param>
    public void AddIncorrectTestSuitElementsNames(List<string> invalidTestSuitElementsNames)
        => this.InvalidTestSuitElementsNames = invalidTestSuitElementsNames;
}
