namespace Eventify.Application.Common.Exceptions
{
    public class ForbiddenAccessException : ApplicationException
    {
        public ForbiddenAccessException() : base("Access forbidden.") { }
        public ForbiddenAccessException(string message) : base(message) { }
    }
}
