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
                var methods = classType.GetTypeInfo().DeclaredMethods;
                var (beforeClassElements, incorrectBeforeClassElementsNames) = GetTestSuiteElements(
                        GetMethodsWithAttribute(methods, typeof(BeforeClassAttribute)),
                        TestSuiteElements.TestSuitElementType.BeforeClass);
                RunBeforeClassElements(beforeClassElements);
                var (tests, incorrectTestsNames) = GetTests(GetMethodsWithAttribute(methods, typeof(TestAttribute)), constructorInfo);
                var (beforeElements, incorrectBeforeElementsNames) = GetTestSuiteElements(
                        GetMethodsWithAttribute(methods, typeof(BeforeAttribute)),
                        TestSuiteElements.TestSuitElementType.Before);
                var (afterElements, incorrectAfterElementsNames) = GetTestSuiteElements(
                        GetMethodsWithAttribute(methods, typeof(AfterAttribute)),
                        TestSuiteElements.TestSuitElementType.After);
                var (afterClassElements, incorrectAfterClassElementsNames) = GetTestSuiteElements(
                        GetMethodsWithAttribute(methods, typeof(AfterClassAttribute)),
                        TestSuiteElements.TestSuitElementType.AfterClass);
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
                    GetThreadSafeCollection(beforeElements),
                    GetThreadSafeCollection(afterElements),
                    beforeClassElements,
                    afterClassElements,
                    incorrectTestSuitElementsNames);
                var testClassInfo = new TestClassInfo(classType.Name);
                testClassInfo.AddIncorrectTestsNames(testSuitStorage.IncorrectFormatTestsNames);
                testClassInfo.AddIncorrectTestSuitElementsNames(testSuitStorage.IncorrectFormatTestSuitElementsNames);
                RunTestSuite(testSuitStorage, testClassInfo);
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

    private static int CalculateTestsNumber(IEnumerable<TestInfo> testsInformation, TestStatus.Status requiredTestStatus)
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

    private static void RunBeforeClassElements(List<TestSuiteElement> beforeClassElements)
    {
        foreach (var beforeClassElement in beforeClassElements)
        {
            beforeClassElement.Run();
        }
    }

    private static void RunTestSuite(TestSuitStorage testSuiteStorage, TestClassInfo testClassInfo)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();

        Parallel.ForEach(testSuiteStorage.Tests, (test) =>
        {
            foreach (var beforeElement in testSuiteStorage.BeforeElements)
            {
                beforeElement.Run(test.ClassObject);
            }

            testClassInfo.AddTest(test.Run());

            foreach (var afterElement in testSuiteStorage.AfterElements)
            {
                afterElement.Run(test.ClassObject);
            }
        });

        foreach (var afterClassElement in testSuiteStorage.AfterClassElements)
        {
            afterClassElement.Run();
        }

        stopWatch.Stop();
        testClassInfo.Duration = stopWatch.ElapsedMilliseconds;
    }

    private static (List<Test> Tests, List<string> IncorrectTestsNames) GetTests(
        IEnumerable<MethodInfo> methodInfoTestsCollection,
        ConstructorInfo? constructorInfo)
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
                var classObject = constructorInfo!.Invoke(new object[] { });
                var test = new Test(methodInfo, classObject);
                tests.Add(test);
            }
        }

        return (tests, incorrectTestsNames);
    }

    private static ConcurrentBag<TestSuiteElement> GetThreadSafeCollection(IEnumerable<TestSuiteElement> collection)
        => new ConcurrentBag<TestSuiteElement>(collection);

    private static (List<TestSuiteElement> TestSuiteElements, List<string> IncorrectTestSuiteElementsNames) GetTestSuiteElements(
        IEnumerable<MethodInfo> methodInfoSuitElementsCollection,
        TestSuiteElements.TestSuitElementType elementsType)
    {
        var testSuitElements = new List<TestSuiteElement>();
        var incorrectTestSuitElementsNames = new List<string>();
        var beforeAndAfterCondition = (MethodInfo methodInfo)
            => methodInfo.GetParameters().Length == 0
            && methodInfo.ReturnType == typeof(void);
        var beforeClassAndAfterClassCondition = (MethodInfo methodInfo)
            => methodInfo.GetParameters().Length == 0
            && methodInfo.ReturnType == typeof(void)
            && methodInfo.IsStatic;
        var condition = elementsType == TestSuiteElements.TestSuitElementType.Before
            || elementsType == TestSuiteElements.TestSuitElementType.After
            ? beforeAndAfterCondition : beforeClassAndAfterClassCondition;
        foreach (var methodInfo in methodInfoSuitElementsCollection)
        {
            if (condition(methodInfo))
            {
                var testSuitElement = new TestSuiteElement(methodInfo, elementsType);
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
