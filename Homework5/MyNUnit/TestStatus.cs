namespace MyNUnit;

/// <summary>
/// Provides information about the test execution statuses.
/// </summary>
public class TestStatus
{
    /// <summary>
    /// Possible test execution status.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// The test was completed successfully.
        /// </summary>
        Passed,

        /// <summary>
        /// The test was completed unsuccessfully.
        /// </summary>
        Failed,

        /// <summary>
        /// The test wasn't started.
        /// </summary>
        Ignored,
    }
}
