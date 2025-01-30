using System.Text.Json.Serialization;

namespace SharedLib.Domain.Responses
{
    public class Response<TData>
    {
        [JsonIgnore]
        public readonly int StatusCode;
        public const int DEFAULT_STATUS_CODE = 200;

        public Response() { }

        public Response(
            TData? data,
            int? code = null,
            string? message = null,
            string[]? errors = null)
        {
            StatusCode = code ?? DEFAULT_STATUS_CODE;
            Data = data;
            Message = message;
            Errors = errors;
        }

        public TData? Data { get; set; }
        public string? Message { get; }
        public string[]? Errors { get; }
        public bool IsSuccess =>
            StatusCode is >= DEFAULT_STATUS_CODE and <= 299;
    }
}
