namespace MyNUnit;

using System.Reflection;

public class TestSuitElement
{
    private readonly object classObject;
    private readonly TestSuitElements.TestSuitElementType type;
    private readonly MethodInfo methodInfo;

    public TestSuitElement(MethodInfo methodInfo, TestSuitElements.TestSuitElementType elementType, object classObject)
    {
        this.methodInfo = methodInfo;
        this.type = elementType;
        this.classObject = classObject;
    }

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
