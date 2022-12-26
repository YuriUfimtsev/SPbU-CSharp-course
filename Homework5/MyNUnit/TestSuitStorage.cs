namespace MyNUnit;

/// <summary>
/// Stores information about the test suit.
/// </summary>
public class TestSuitStorage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestSuitStorage"/> class.
    /// </summary>
    /// <param name="tests">Tests.</param>
    /// <param name="incorrectTestsNames">Names of the test methods
    /// with the wrong return type or the wrong number of parameters.</param>
    /// <param name="beforeElements">Test suit elements corresponding to the Before attribute.</param>
    /// <param name="afterElements">Test suit elements corresponding to the After attribute.</param>
    /// <param name="beforeClassElements">Test suit elements corresponding to the BeforeClass attribute.</param>
    /// <param name="afterClassElements">Test suit elements corresponding to the AfterClass attribute.</param>
    /// <param name="incorrectTestSuitElementsNames">Names of the wrong format methods from the test suit.</param>
    public TestSuitStorage(
        List<Test> tests,
        List<string> incorrectTestsNames,
        List<TestSuitElement> beforeElements,
        List<TestSuitElement> afterElements,
        List<TestSuitElement> beforeClassElements,
        List<TestSuitElement> afterClassElements,
        List<string> incorrectTestSuitElementsNames)
    {
        this.Tests = tests;
        this.BeforeElements = beforeElements;
        this.AfterElements = afterElements;
        this.BeforeClassElements = beforeClassElements;
        this.AfterClassElements = afterClassElements;
        this.IncorrectFormatTestsNames = incorrectTestsNames;
        this.IncorrectFormatTestSuitElementsNames = incorrectTestSuitElementsNames;
    }

    /// <summary>
    /// Gets the tests.
    /// </summary>
    public List<Test> Tests { get; }

    /// <summary>
    /// Gets the names of the wrong format test methods.
    /// </summary>
    public List<string> IncorrectFormatTestsNames { get; private set; }

    /// <summary>
    /// Gets the test suit elements corresponding to the Before attribute.
    /// </summary>
    public List<TestSuitElement> BeforeElements { get; }

    /// <summary>
    /// Gets the test suit elements corresponding to the After attribute.
    /// </summary>
    public List<TestSuitElement> AfterElements { get; }

    /// <summary>
    /// Gets the test suit elements corresponding to the BeforeClass attribute.
    /// </summary>
    public List<TestSuitElement> BeforeClassElements { get; }

    /// <summary>
    /// Gets the test suit elements corresponding to the AfterClass attribute.
    /// </summary>
    public List<TestSuitElement> AfterClassElements { get; }

    /// <summary>
    /// Gets the names of the wrong format test suit methods.
    /// </summary>
    public List<string> IncorrectFormatTestSuitElementsNames { get; private set; }
}
