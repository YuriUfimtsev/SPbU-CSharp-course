namespace MyNUnit;

using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using MyNUnit.Information;
using MyNUnit.Attributes;
using System.Diagnostics;

public static class TestRunner
{
    public static List<TestClassInfo> RunTests(string pathToSource)
    {
        var assembliesTestsInfo = new List<TestClassInfo>();
        var assemblies = Directory.EnumerateFiles(pathToSource, "*.dll");
        for (var i = 0; i < assemblies.Count(); ++i)
        {
            var classes = Assembly.Load(assemblies.ElementAt(i)).ExportedTypes.Where(type => type.IsClass);
            for (var j = 0; j < classes.Count(); ++j)
            {
                var classType = classes.ElementAt(i);
                var testClassInfo = new TestClassInfo(classType.Name);
                var constructorInfo = classType.GetConstructor(Type.EmptyTypes);
                var classObject = constructorInfo!.Invoke(new object[] { });
                var methods = classType.GetTypeInfo().DeclaredMethods;
                var testSuitStorage = new SmartTestSuitStorage(
                    GetTests(GetMethodsWithAttribute(methods, typeof(TestAttribute)), classObject),
                    GetTestSuitElements(
                        GetMethodsWithAttribute(methods, typeof(BeforeAttribute)),
                        TestSuitElements.TestSuitElementType.Before,
                        classObject),
                    GetTestSuitElements(
                        GetMethodsWithAttribute(methods, typeof(AfterAttribute)),
                        TestSuitElements.TestSuitElementType.After,
                        classObject),
                    GetTestSuitElements(
                        GetMethodsWithAttribute(methods, typeof(BeforeClassAttribute)),
                        TestSuitElements.TestSuitElementType.BeforeClass,
                        classObject),
                    GetTestSuitElements(
                        GetMethodsWithAttribute(methods, typeof(AfterClassAttribute)),
                        TestSuitElements.TestSuitElementType.AfterClass,
                        classObject));
                RunTestSuit(testSuitStorage, testClassInfo);
                assembliesTestsInfo.Add(testClassInfo);
            }
        }

        return assembliesTestsInfo;
    }

    private static void RunTestSuit(SmartTestSuitStorage testSuitStorage, TestClassInfo testClassInfo)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        foreach (var beforeClassElement in testSuitStorage.BeforeClassElements)
        {
            testClassInfo.AddTestSuitElement(beforeClassElement.Run(), TestSuitElements.TestSuitElementType.BeforeClass);
        }

        foreach (var test in testSuitStorage.Tests)
        {
            foreach (var beforeElement in testSuitStorage.BeforeElements)
            {
                testClassInfo.AddTestSuitElement(beforeElement.Run(), TestSuitElements.TestSuitElementType.Before);
            }

            testClassInfo.AddTest(test.Run());

            foreach (var afterElement in testSuitStorage.AfterElements)
            {
                testClassInfo.AddTestSuitElement(afterElement.Run(), TestSuitElements.TestSuitElementType.After);
            }
        }

        foreach (var afterClassElement in testSuitStorage.AfterClassElements)
        {
            testClassInfo.AddTestSuitElement(afterClassElement.Run(), TestSuitElements.TestSuitElementType.AfterClass);
        }

        stopWatch.Stop();
        testClassInfo.Duration = stopWatch.ElapsedMilliseconds;
    }

    private static List<Test> GetTests(IEnumerable<MethodInfo> methodInfoTestsCollection, object classObject)
    {
        var tests = new List<Test>();
        foreach (var methodInfo in methodInfoTestsCollection)
        {
            var test = new Test(methodInfo, classObject);
            tests.Add(test);
        }

        return tests;
    }

    private static List<TestSuitElement> GetTestSuitElements(
        IEnumerable<MethodInfo> methodInfoSuitElementsCollection,
        TestSuitElements.TestSuitElementType elementsType,
        object classObject)
    {
        var testSuitElements = new List<TestSuitElement>();
        foreach (var methodInfo in methodInfoSuitElementsCollection)
        {
            var testSuitElement = new TestSuitElement(methodInfo, elementsType, classObject);
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
