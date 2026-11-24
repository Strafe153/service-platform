namespace Users.Application.Exceptions;

public class DatabaseException : Exception
{
    public DatabaseException() : base()
    {
    }

    public DatabaseException(string message) : base(message)
    {
    }

    public DatabaseException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public DatabaseException(Exception exception) : this(exception.Message, exception)
    {
    }
}