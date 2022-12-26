namespace SimpleFTPClient;

/// <summary>
/// Throws if the network member tries to process an incorrect path.
/// </summary>
public class InvalidPathException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPathException"/> class.
    /// </summary>
    public InvalidPathException()
        : base()
    {
    }
}