namespace MyNUnit.Attributes;

/// <summary>
/// Implements the attribute for a method that should be called before running each class test.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BeforeAttribute"/> class.
    /// </summary>
    public BeforeAttribute()
    {
    }
}
