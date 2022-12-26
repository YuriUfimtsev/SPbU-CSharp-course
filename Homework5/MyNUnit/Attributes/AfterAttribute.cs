namespace MyNUnit.Attributes;

/// <summary>
/// Implements the attribute for a method that should be called after running each class test.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AfterAttribute"/> class.
    /// </summary>
    public AfterAttribute()
    {
    }
}
