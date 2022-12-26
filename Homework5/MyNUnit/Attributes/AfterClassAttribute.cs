namespace MyNUnit.Attributes;

/// <summary>
/// Implements the attribute for a method that should be called after running all class tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterClassAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AfterClassAttribute"/> class.
    /// </summary>
    public AfterClassAttribute()
    {
    }
}
