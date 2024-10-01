namespace BuildingBlock.Exceptions
{
    public class ExternalApiException : Exception
    {

        public ExternalApiException(string message) : base(message) { }

        public ExternalApiException(string message, string details) : base(message + ". " + details)
        {

        }
    }
}
