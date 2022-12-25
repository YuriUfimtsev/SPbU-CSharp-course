namespace MyNUnit;

using MyNUnit.Information;
using System.Reflection;
using MyNUnit.Attributes;
using System.Diagnostics;

public class TestSuitElement
{
    private object classObject;

    public TestSuitElement(MethodInfo methodInfo, TestSuitElements.TestSuitElementType elementType, object classObject)
    {
        this.Info = methodInfo;
        this.Type = elementType;
        this.classObject = classObject;
    }

    public TestSuitElements.TestSuitElementType Type { get; }

    public MethodInfo Info { get; }

    public TestSuitElementInfo Run()
    {
        if (this.Type == TestSuitElements.TestSuitElementType.Before
            || this.Type == TestSuitElements.TestSuitElementType.After)
        {
            this.Info.Invoke(this.classObject, new object[] { });

            // ...
        }

        this.Info.Invoke(null, new object[] { });

        // ...
    }

    public void RunWithoutReport()
    {
    }
}
