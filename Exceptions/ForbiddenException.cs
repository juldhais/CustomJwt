namespace CustomJwt.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Forbidden")
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }
}