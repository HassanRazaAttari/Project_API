namespace Project_API.Models.Entities
{
    public class ApiResponse
    {
        public bool isSuccess { get; set; }
        public string statusCode { get; set; }
        public object message { get; set; }
        public object data { get; set; }
        public IEnumerable<string> errors { get; set; }
    }
}
