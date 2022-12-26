namespace MyNUnit.Attributes;

/// <summary>
/// Implements the attribute for a test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    public TestAttribute()
        : this(null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    /// <param name="expectedException">The type of exception that is expected to be thrown in test method.</param>
    public TestAttribute(Type expectedException)
    : this(expectedException, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    /// <param name="reasonForIgnoring">The reason why the test method shouldn't be run.</param>
    public TestAttribute(string reasonForIgnoring)
    : this(null, reasonForIgnoring)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    /// <param name="expectedException">The type of exception that is expected to be thrown in test method.</param>
    /// <param name="reasonForIgnoring">The reason why the test method shouldn't be run.</param>
    public TestAttribute(Type? expectedException, string? reasonForIgnoring)
    {
        this.Expected = expectedException;
        this.Ignore = reasonForIgnoring;
    }

    /// <summary>
    /// Gets or sets the type of exception that is expected to be thrown in test method.
    /// </summary>
    public Type? Expected { get; set; }

    /// <summary>
    /// Gets or sets the reason why the test method shouldn't be run.
    /// </summary>
    public string? Ignore { get; set; }
}
