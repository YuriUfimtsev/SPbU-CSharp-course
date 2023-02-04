namespace SimpleFTPClient;

/// <summary>
/// Throws if the client receives an incorrect response from the server.
/// </summary>
public class InvalidServerResponseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidServerResponseException"/> class.
    /// </summary>
    public InvalidServerResponseException()
        : base()
    {
    }
}
