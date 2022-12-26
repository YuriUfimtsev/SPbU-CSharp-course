namespace MyNUnit;

using System.Reflection;

/// <summary>
/// Implements the test suit method entitie.
/// </summary>
public class TestSuitElement
{
    private readonly object classObject;
    private readonly TestSuitElements.TestSuitElementType type;
    private readonly MethodInfo methodInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestSuitElement"/> class.
    /// </summary>
    /// <param name="methodInfo">Method information.</param>
    /// <param name="elementType">Type of the test suit element.</param>
    /// <param name="classObject">Object of the class to which test method belongs.</param>
    public TestSuitElement(MethodInfo methodInfo, TestSuitElements.TestSuitElementType elementType, object classObject)
    {
        this.methodInfo = methodInfo;
        this.type = elementType;
        this.classObject = classObject;
    }

    /// <summary>
    /// Runs the test suit element.
    /// </summary>
    public void Run()
    {
        try
        {
            if (this.type == TestSuitElements.TestSuitElementType.Before
                || this.type == TestSuitElements.TestSuitElementType.After)
            {
                this.methodInfo.Invoke(this.classObject, new object[] { });
                return;
            }

            this.methodInfo.Invoke(null, new object[] { });
            return;
        }
        catch (Exception)
        {
            return;
        }
    }
}
