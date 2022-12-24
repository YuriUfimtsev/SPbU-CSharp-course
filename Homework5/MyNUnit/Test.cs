namespace MyNUnit;

using System.Reflection;
using MyNUnit.Information;

public class Test
{
    public Test(MethodInfo methodInfo)
    {
        this.Info = methodInfo;
    }

    public MethodInfo Info { get; }

    public TestInfo Run()
    {
    }
}
