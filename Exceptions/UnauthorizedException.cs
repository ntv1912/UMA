namespace UMA.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() : base("You do not have permission to perform this action.") { }
    }
}
