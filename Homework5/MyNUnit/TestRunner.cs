namespace MyNUnit;

using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using MyNUnit.Information;
using MyNUnit.Attributes;
using System.Diagnostics;
using System.Collections.Concurrent;

/// <summary>
/// Implements running assemblies tests and providing information about them.
/// </summary>
public static class TestRunner
{
    /// <summary>
    /// Runs the test.
    /// </summary>
    /// <param name="pathToSource">Path to the assemblies containts test suits.</param>
    /// <returns>Information about running the test.</returns>
    public static List<TestClassInfo> RunTests(string pathToSource)
    {
        var testClassesInfo = new ConcurrentBag<TestClassInfo>();
        var assembliesPaths = Directory.EnumerateFiles(pathToSource, "*.dll");
        Parallel.ForEach(assembliesPaths, (assemblyPath) =>
        {
            var classesTypes = Assembly.LoadFrom(assemblyPath).ExportedTypes.Where(type => type.IsClass);
            Parallel.ForEach(classesTypes, (classType) =>
            {
                var constructorInfo = classType.GetConstructor(Type.EmptyTypes);
                var classObject = constructorInfo!.Invoke(new object[] { });
                var methods = classType.GetTypeInfo().DeclaredMethods;
                var (tests, incorrectTestsNames) = GetTests(GetMethodsWithAttribute(methods, typeof(TestAttribute)), classObject);
                var (beforeElements, incorrectBeforeElementsNames) = GetTestSuitElements(
                        GetMethodsWithAttribute(methods, typeof(BeforeAttribute)),
                        TestSuitElements.TestSuitElementType.Before,
                        classObject);
                var (afterElements, incorrectAfterElementsNames) = GetTestSuitElements(
                        GetMethodsWithAttribute(methods, typeof(AfterAttribute)),
                        TestSuitElements.TestSuitElementType.After,
                        classObject);
                var (beforeClassElements, incorrectBeforeClassElementsNames) = GetTestSuitElements(
                        GetMethodsWithAttribute(methods, typeof(BeforeClassAttribute)),
                        TestSuitElements.TestSuitElementType.BeforeClass,
                        classObject);
                var (afterClassElements, incorrectAfterClassElementsNames) = GetTestSuitElements(
                        GetMethodsWithAttribute(methods, typeof(AfterClassAttribute)),
                        TestSuitElements.TestSuitElementType.AfterClass,
                        classObject);
                var incorrectTestSuitElementsNames = ConcatenateLists(new[]
                {
                    incorrectBeforeElementsNames,
                    incorrectAfterElementsNames,
                    incorrectBeforeClassElementsNames,
                    incorrectAfterClassElementsNames,
                });
                var testSuitStorage = new TestSuitStorage(
                    tests,
                    incorrectTestsNames,
                    beforeElements,
                    afterElements,
                    beforeClassElements,
                    afterClassElements,
                    incorrectTestSuitElementsNames);
                var testClassInfo = new TestClassInfo(classType.Name);
                testClassInfo.AddIncorrectTestsNames(testSuitStorage.IncorrectFormatTestsNames);
                testClassInfo.AddIncorrectTestSuitElementsNames(testSuitStorage.IncorrectFormatTestSuitElementsNames);
                RunTestSuit(testSuitStorage, testClassInfo);
                testClassesInfo.Add(testClassInfo);
            });
        });

        return testClassesInfo.ToList();
    }

    /// <summary>
    /// Generates the report about the test run based on the TestClassInfo.
    /// </summary>
    /// <param name="testClassInformation">Information about the test run.</param>
    /// <param name="userWriter">Destination for submitting the report.</param>
    public static void GenerateTestRunnerReport(List<TestClassInfo> testClassInformation, TextWriter userWriter)
    {
        foreach (var testClass in testClassInformation)
        {
            userWriter.WriteLine("------------------------");
            userWriter.WriteLine($"Class: {testClass.TestClassName}");
            userWriter.WriteLine($"Tests execution time: {testClass.Duration} ms");
            userWriter.WriteLine($"Passed: {CalculateTestsNumber(testClass.TestsInfo, TestStatus.Status.Passed)}");
            userWriter.WriteLine($"Failed: {CalculateTestsNumber(testClass.TestsInfo, TestStatus.Status.Failed)}");
            userWriter.WriteLine($"Ignored: {CalculateTestsNumber(testClass.TestsInfo, TestStatus.Status.Ignored)}");
            foreach (var test in testClass.TestsInfo)
            {
                userWriter.WriteLine("------------------------");
                userWriter.WriteLine($"Name: {test.Name}, Duration: {test.Duration} ms, Status: {test.ExecutionResult}. {test.Report}");
            }

            userWriter.WriteLine("------------------------");
            if (testClass.InvalidTestsNames.Count > 0)
            {
                userWriter.Write("Сheck the number of arguments and the type of return value of the test methods: ");
                for (var i = 0; i < testClass.InvalidTestsNames.Count; ++i)
                {
                    userWriter.Write($"{testClass.InvalidTestsNames[i]}");
                    userWriter.Write(i == testClass.InvalidTestsNames.Count - 1 ? "." : ", ");
                }

                userWriter.WriteLine();
            }

            if (testClass.InvalidTestSuitElementsNames.Count > 0)
            {
                userWriter.Write("Сheck the number of arguments and the type of return value of the test suit methods: ");
                for (var i = 0; i < testClass.InvalidTestSuitElementsNames.Count; ++i)
                {
                    userWriter.Write($"{testClass.InvalidTestSuitElementsNames[i]}");
                    userWriter.Write(i == testClass.InvalidTestSuitElementsNames.Count - 1 ? "." : ", ");
                }

                userWriter.WriteLine();
            }
        }
    }

    private static int CalculateTestsNumber(List<TestInfo> testsInformation, TestStatus.Status requiredTestStatus)
    {
        var satisfyingTestsNumber = 0;
        foreach (var test in testsInformation)
        {
            if (test.ExecutionResult == requiredTestStatus)
            {
                ++satisfyingTestsNumber;
            }
        }

        return satisfyingTestsNumber;
    }

    private static void RunTestSuit(TestSuitStorage testSuitStorage, TestClassInfo testClassInfo)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        foreach (var beforeClassElement in testSuitStorage.BeforeClassElements)
        {
            beforeClassElement.Run();
        }

        foreach (var test in testSuitStorage.Tests)
        {
            foreach (var beforeElement in testSuitStorage.BeforeElements)
            {
                beforeElement.Run();
            }

            testClassInfo.AddTest(test.Run());

            foreach (var afterElement in testSuitStorage.AfterElements)
            {
                afterElement.Run();
            }
        }

        foreach (var afterClassElement in testSuitStorage.AfterClassElements)
        {
            afterClassElement.Run();
        }

        stopWatch.Stop();
        testClassInfo.Duration = stopWatch.ElapsedMilliseconds;
    }

    private static (List<Test> Tests, List<string> IncorrectTestsNames) GetTests(
        IEnumerable<MethodInfo> methodInfoTestsCollection,
        object classObject)
    {
        var tests = new List<Test>();
        var incorrectTestsNames = new List<string>();
        foreach (var methodInfo in methodInfoTestsCollection)
        {
            if (methodInfo.GetParameters().Length > 0 || methodInfo.ReturnType != typeof(void))
            {
                incorrectTestsNames.Add(methodInfo.Name);
            }
            else
            {
                var test = new Test(methodInfo, classObject);
                tests.Add(test);
            }
        }

        return (tests, incorrectTestsNames);
    }

    private static (List<TestSuitElement> TestSuitElements, List<string> IncorrectTestSuitElementsNames) GetTestSuitElements(
        IEnumerable<MethodInfo> methodInfoSuitElementsCollection,
        TestSuitElements.TestSuitElementType elementsType,
        object classObject)
    {
        var testSuitElements = new List<TestSuitElement>();
        var incorrectTestSuitElementsNames = new List<string>();
        var beforeAndAfterCondition = (MethodInfo methodInfo)
            => methodInfo.GetParameters().Length == 0
            && methodInfo.ReturnType == typeof(void);
        var beforeClassAndAfterClassCondition = (MethodInfo methodInfo)
            => methodInfo.GetParameters().Length == 0
            && methodInfo.ReturnType == typeof(void)
            && methodInfo.IsStatic;
        var condition = elementsType == TestSuitElements.TestSuitElementType.Before
            || elementsType == TestSuitElements.TestSuitElementType.After
            ? beforeAndAfterCondition : beforeClassAndAfterClassCondition;
        foreach (var methodInfo in methodInfoSuitElementsCollection)
        {
            if (condition(methodInfo))
            {
                var testSuitElement = new TestSuitElement(methodInfo, elementsType, classObject);
                testSuitElements.Add(testSuitElement);
            }
            else
            {
                incorrectTestSuitElementsNames.Add(methodInfo.Name);
            }
        }

        return (testSuitElements, incorrectTestSuitElementsNames);
    }

    private static IEnumerable<MethodInfo> GetMethodsWithAttribute(
        IEnumerable<MethodInfo> methodsCollection,
        Type attributeType)
        =>
        from method in methodsCollection
        where method.IsDefined(attributeType)
        select method;

    private static List<string> ConcatenateLists(List<string>[] lists)
    {
        var size = 0;
        for (var i = 0; i < lists.Length; ++i)
        {
            size += lists[i].Count;
        }

        var list = new List<string>(size);
        for (var i = 0; i < lists.Length; ++i)
        {
            list.AddRange(lists[i]);
        }

        return list;
    }
}
