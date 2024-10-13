namespace IfinionBackendAssessment.DataAccess.Common
{
    public class ApiResponse<T>
    {
        public ApiResponse(string message, T data = default)
        {
            Message = message;
            Data = data;
        }
        public ApiResponse()
        {

        }

        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public string[] Errors { get; set; } = [];
        public T Data { get; set; }
    }
}
