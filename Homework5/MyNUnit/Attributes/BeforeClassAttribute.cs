namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : Attribute
{
    public BeforeClassAttribute()
    {
    }
}
