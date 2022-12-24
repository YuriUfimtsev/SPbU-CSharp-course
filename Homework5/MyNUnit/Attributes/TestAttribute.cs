namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    private Exception? expectedException;
    private string? reasonForIgnoring;

    public TestAttribute()
        : this(null, null)
    {
    }

    public TestAttribute(Exception expectedException)
    : this(expectedException, null)
    {
    }

    public TestAttribute(string reasonForIgnoring)
    : this(null, reasonForIgnoring)
    {
    }

    public TestAttribute(Exception? expectedException, string? reasonForIgnoring)
    {
        this.expectedException = expectedException;
        this.reasonForIgnoring = reasonForIgnoring;
    }

    public bool IsIgnored => reasonForIgnoring != null;
}
