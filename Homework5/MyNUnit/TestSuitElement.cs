namespace MyNUnit;

using System.Reflection;

/// <summary>
/// Implements the test suit method entitie.
/// </summary>
public class TestSuiteElement
{
    private readonly TestSuiteElements.TestSuitElementType type;
    private readonly MethodInfo methodInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestSuiteElement"/> class.
    /// </summary>
    /// <param name="methodInfo">Method information.</param>
    /// <param name="elementType">Type of the test suit element.</param>
    public TestSuiteElement(MethodInfo methodInfo, TestSuiteElements.TestSuitElementType elementType)
    {
        this.methodInfo = methodInfo;
        this.type = elementType;
    }

    /// <summary>
    /// Runs the test suit element.
    /// </summary>
    /// <param name="classObject">Object of the class to which test suit element belongs.</param>
    public void Run(object? classObject = null)
    {
        try
        {
            if (this.type == TestSuiteElements.TestSuitElementType.Before
                || this.type == TestSuiteElements.TestSuitElementType.After)
            {
                this.methodInfo.Invoke(classObject, new object[] { });
                return;
            }

            this.methodInfo.Invoke(null, new object[] { });
        }
        catch (Exception)
        {
            return;
        }
    }
}
