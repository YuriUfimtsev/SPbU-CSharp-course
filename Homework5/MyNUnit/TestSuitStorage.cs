namespace MyNUnit;

public class TestSuitStorage
{
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

    public List<Test> Tests { get; }

    public List<string> IncorrectFormatTestsNames { get; private set; }

    public List<TestSuitElement> BeforeElements { get; }

    public List<TestSuitElement> AfterElements { get; }

    public List<TestSuitElement> BeforeClassElements { get; }

    public List<TestSuitElement> AfterClassElements { get; }

    public List<string> IncorrectFormatTestSuitElementsNames { get; private set; }
}
