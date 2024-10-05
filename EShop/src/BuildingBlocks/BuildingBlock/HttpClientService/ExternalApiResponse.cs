namespace BuildingBlock.HttpClientService
{
    public class ExternalApiResponse
    {
        public string Status { get; set; }
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalResults { get; set; }

        public Object Data { get; set; }
    }
}
