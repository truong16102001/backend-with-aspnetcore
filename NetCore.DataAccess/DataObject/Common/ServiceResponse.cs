namespace NetCore.DataAccess.DataObject.Common
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }

        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }
    }
}
