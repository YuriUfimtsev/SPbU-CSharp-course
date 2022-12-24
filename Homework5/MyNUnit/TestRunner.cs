namespace MyNUnit;

using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using MyNUnit.Information;
using MyNUnit.Attributes;

public static class TestRunner
{
    public static List<TestClassInfo> RunTests(string pathToSource)
    {
        var assemblies = Directory.EnumerateFiles(pathToSource, "*.dll");
        for (var i = 0; i < assemblies.Count(); ++i)
        {
            var classes = Assembly.Load(assemblies.ElementAt(i)).ExportedTypes.Where(type => type.IsClass);
            for (var j = 0; j < classes.Count(); ++j)
            {
                var methods = classes.ElementAt(i).GetTypeInfo().DeclaredMethods;
                var testStorage = new SmartTestSuitStorage(
                    GetTests(GetMethodsWithAttribute(methods, typeof(TestAttribute))),
                    GetTestSuitElements(GetMethodsWithAttribute(methods, typeof(BeforeAttribute)), TestSuitElements.TestSuitElementType.Before),
                    GetTestSuitElements(GetMethodsWithAttribute(methods, typeof(AfterAttribute)), TestSuitElements.TestSuitElementType.After),
                    GetTestSuitElements(GetMethodsWithAttribute(methods, typeof(BeforeClassAttribute)), TestSuitElements.TestSuitElementType.BeforeClass),
                    GetTestSuitElements(GetMethodsWithAttribute(methods, typeof(AfterClassAttribute)), TestSuitElements.TestSuitElementType.AfterClass));
            }
        }
    }

    private static List<Test> GetTests(IEnumerable<MethodInfo> methodInfoTestsCollection)
    {
        var tests = new List<Test>();
        foreach (var methodInfo in methodInfoTestsCollection)
        {
            var test = new Test(methodInfo);
            tests.Add(test);
        }

        return tests;
    }

    private static List<TestSuitElement> GetTestSuitElements(
        IEnumerable<MethodInfo> methodInfoSuitElementsCollection,
        TestSuitElements.TestSuitElementType elementsType)
    {
        var testSuitElements = new List<TestSuitElement>();
        foreach (var methodInfo in methodInfoSuitElementsCollection)
        {
            var testSuitElement = new TestSuitElement(methodInfo, elementsType);
            testSuitElements.Add(testSuitElement);
        }

        return testSuitElements;
    }

    private static IEnumerable<MethodInfo> GetMethodsWithAttribute(
        IEnumerable<MethodInfo> methodsCollection,
        Type attributeType)
        =>
        from method in methodsCollection
        where method.IsDefined(attributeType)
        select method;
}
