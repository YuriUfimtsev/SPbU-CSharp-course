namespace MyNUnit;

public class SmartTestSuitStorage
{
    public SmartTestSuitStorage(
        List<Test> tests,
        List<TestSuitElement> before,
        List<TestSuitElement> after,
        List<TestSuitElement> beforeClass,
        List<TestSuitElement> afterClass)
    {
        this.Tests = tests;
        this.BeforeElements = before;
        this.AfterElements = after;
        this.BeforeClassElements = beforeClass;
        this.AfterClassElements = afterClass;
        this.IncorrectFormatTests = new List<Test>();
        this.IncorrectFormatBeforeElements = new List<TestSuitElement>();
        this.IncorrectFormatAfterElements = new List<TestSuitElement>();
        this.IncorrectFormatBeforeClassElements = new List<TestSuitElement>();
        this.IncorrectFormatAfterClassElements = new List<TestSuitElement>();
        this.RunTestSuitReview();
    }

    public List<Test> Tests { get; }

    public List<TestSuitElement> BeforeElements { get; }

    public List<TestSuitElement> AfterElements { get; }

    public List<TestSuitElement> BeforeClassElements { get; }

    public List<TestSuitElement> AfterClassElements { get; }

    public List<Test> IncorrectFormatTests { get; private set; }

    public List<TestSuitElement> IncorrectFormatBeforeElements { get; private set; }

    public List<TestSuitElement> IncorrectFormatAfterElements { get; private set; }

    public List<TestSuitElement> IncorrectFormatBeforeClassElements { get; private set; }

    public List<TestSuitElement> IncorrectFormatAfterClassElements { get; private set; }

    public void RunTestSuitReview()
    {
        this.RunTestsReview();
        this.RunTestSuitElementsReview(TestSuitElements.TestSuitElementType.Before);
        this.RunTestSuitElementsReview(TestSuitElements.TestSuitElementType.After);
        this.RunTestSuitElementsReview(TestSuitElements.TestSuitElementType.BeforeClass);
        this.RunTestSuitElementsReview(TestSuitElements.TestSuitElementType.AfterClass);
    }

    private void RunTestsReview()
    {
        var incorrectFormatTests = new List<Test>();
        foreach (var test in this.Tests)
        {
            if (test.Info.GetParameters().Length > 0 || test.Info.ReturnType != typeof(void))
            {
                incorrectFormatTests.Add(test);
                this.Tests.Remove(test);
            }
        }

        this.IncorrectFormatTests = incorrectFormatTests;
    }

    private void RunTestSuitElementsReview(TestSuitElements.TestSuitElementType type)
    {
        List<TestSuitElement> storage = new ();
        List<TestSuitElement> incorrectFormatTestSuitElementsStorage = new ();
        Func<TestSuitElement, bool>? condition = default;
        var beforeAndAfterCondition = (TestSuitElement testSuitElement)
            => testSuitElement.Info.GetParameters().Length > 0
            || testSuitElement.Info.ReturnType != typeof(void);
        var beforeClassAndAfterClassCondition = (TestSuitElement testSuitElement)
            => testSuitElement.Info.GetParameters().Length > 0
            || testSuitElement.Info.ReturnType != typeof(void)
            || !testSuitElement.Info.IsStatic;
        switch (type)
        {
            case TestSuitElements.TestSuitElementType.Before:
                storage = this.BeforeElements;
                condition = beforeAndAfterCondition;
                incorrectFormatTestSuitElementsStorage = this.IncorrectFormatBeforeElements;
                break;
            case TestSuitElements.TestSuitElementType.After:
                storage = this.AfterElements;
                condition = beforeAndAfterCondition;
                incorrectFormatTestSuitElementsStorage = this.IncorrectFormatAfterElements;
                break;
            case TestSuitElements.TestSuitElementType.BeforeClass:
                storage = this.BeforeClassElements;
                condition = beforeClassAndAfterClassCondition;
                incorrectFormatTestSuitElementsStorage = this.IncorrectFormatBeforeClassElements;
                break;
            case TestSuitElements.TestSuitElementType.AfterClass:
                storage = this.AfterClassElements;
                condition = beforeClassAndAfterClassCondition;
                incorrectFormatTestSuitElementsStorage = this.IncorrectFormatAfterClassElements;
                break;
        }

        var incorrectFormatTestSuitElements = new List<TestSuitElement>();
        foreach (var element in storage)
        {
            if (condition!(element))
            {
                incorrectFormatTestSuitElements.Add(element);
                storage.Remove(element);
            }
        }

        incorrectFormatTestSuitElementsStorage = incorrectFormatTestSuitElements;
    }
}
