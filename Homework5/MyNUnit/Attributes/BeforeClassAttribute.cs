namespace MyNUnit.Attributes;

/// <summary>
/// Implements the attribute for a method that should be called before running all class tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BeforeClassAttribute"/> class.
    /// </summary>
    public BeforeClassAttribute()
    {
    }
}
