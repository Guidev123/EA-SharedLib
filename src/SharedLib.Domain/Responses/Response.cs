using System.Text.Json.Serialization;

namespace SharedLib.Domain.Responses
{
    public class Response<TData>(
        bool isSucess,
        int code,
        TData? data,
        string? message = null,
        string[]? errors = null
        )
    {
        public const int DEFAULT_STATUS_CODE = 200;
        public TData? Data { get; set; } = data;
        public string? Message { get; } = message;
        public string[]? Errors { get; } = errors;
        public bool IsSuccess { get; set; } = isSucess;
        [JsonIgnore]
        public int Code { get; set; } = code;
    }
}
