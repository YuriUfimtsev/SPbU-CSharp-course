namespace MyNUnit;

/// <summary>
/// Provides information about the test suit elements types.
/// </summary>
public class TestSuiteElements
{
    /// <summary>
    /// Possible test suit element type.
    /// </summary>
    public enum TestSuitElementType
    {
        /// <summary>
        /// Test suit element should be called before running each class test.
        /// </summary>
        Before,

        /// <summary>
        /// Test suit element should be called after running each class test.
        /// </summary>
        After,

        /// <summary>
        /// Test suit element should be called before running all class tests.
        /// </summary>
        BeforeClass,

        /// <summary>
        /// Test suit element should be called after running all class tests.
        /// </summary>
        AfterClass,
    }
}
