using System.Text.Json.Serialization;

namespace SharedLib.Domain.Responses
{
    public class Response<TData>(
        TData? data,
        int? code = null,
        string? message = null,
        string[]? errors = null)
    {
        [JsonIgnore]
        public readonly int statusCode = code ?? DEFAULT_STATUS_CODE;
        private const int DEFAULT_STATUS_CODE = 200;
        public TData? Data { get; set; } = data;
        public string? Message { get; } = message;
        public string[]? Errors { get; } = errors;
        public bool IsSuccess =>
            statusCode is >= DEFAULT_STATUS_CODE and <= 299;
    }
}
