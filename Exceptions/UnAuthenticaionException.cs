namespace UMA.Exceptions
{
    public class UnAuthenticaionException : Exception
    {
        public UnAuthenticaionException() : base("Invalid token or token has expired.") { }
    }
}
