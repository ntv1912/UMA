namespace UMA.Exceptions
{
    public class DuplicateDataException : Exception
    {
        public DuplicateDataException() : base("The data already exists in the system.") { }
    }
}
