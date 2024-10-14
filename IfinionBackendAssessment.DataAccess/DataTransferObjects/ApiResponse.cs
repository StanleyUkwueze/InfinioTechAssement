namespace IfinionBackendAssessment.DataAccess.Common
{
    public class APIResponse<T>
    {
        public APIResponse(string message, T data = default)
        {
            Message = message;
            Data = data;
        }
        public APIResponse()
        {

        }

        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public string[] Errors { get; set; } = [];
        public T Data { get; set; }
    }
}
