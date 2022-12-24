namespace MyNUnit;

using MyNUnit.Information;
using System.Reflection;

public class TestSuitElement
{
    public TestSuitElement(MethodInfo methodInfo, TestSuitElements.TestSuitElementType elementType)
    {
        this.Info = methodInfo;
        this.Type = elementType;
    }

    public TestSuitElements.TestSuitElementType Type { get; }

    public MethodInfo Info { get; }

    public TestInfo Run()
    {
    }

    public void RunWithoutReport()
    {
    }
}
