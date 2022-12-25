namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    public TestAttribute()
        : this(null, null)
    {
    }

    public TestAttribute(Type expectedException)
    : this(expectedException, null)
    {
    }

    public TestAttribute(string reasonForIgnoring)
    : this(null, reasonForIgnoring)
    {
    }

    public TestAttribute(Type? expectedException, string? reasonForIgnoring)
    {
        this.Expected = expectedException;
        this.Ignore = reasonForIgnoring;
    }

    public Type? Expected { get; set; }

    public string? Ignore { get; set; }
}
