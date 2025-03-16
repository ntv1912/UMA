namespace UMA.Exceptions
{
    public class NotFoundException:Exception
    {
        public NotFoundException() : base("User is not exists") { }
    }
}
