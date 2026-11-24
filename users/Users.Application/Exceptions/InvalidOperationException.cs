namespace Users.Application.Exceptions;

public class InvalidOperationException : Exception
{
    public InvalidOperationException() : base()
    {
    }

    public InvalidOperationException(string message) : base(message)
    {
    }

    public InvalidOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}