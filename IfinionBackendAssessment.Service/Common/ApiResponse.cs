namespace IfinionBackendAssessment.DataAccess.Common
{
    public class ApiResponse<T>
    {
        public ApiResponse(int statusCode, string message, T data = default)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
        public ApiResponse()
        {

        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public string[] Errors { get; set; } = [];
        public T Data { get; set; }
    }
}
