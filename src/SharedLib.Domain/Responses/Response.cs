using System.Text.Json.Serialization;

namespace SharedLib.Domain.Responses
{
    public class Response<TData>
    {
        public const int DEFAULT_STATUS_CODE = 200;

        public Response(
            TData? data,
            int code = DEFAULT_STATUS_CODE,
            string? message = null,
            string[]? errors = null)
        {
            Data = data;
            Message = message;
            Code = code;
            Errors = errors;
        }

        [JsonIgnore]
        public int Code { get; }
        public TData? Data { get; set; }
        public string? Message { get; }
        public string[]? Errors { get; }

        [JsonIgnore]
        public bool IsSuccess
            => Code is >= DEFAULT_STATUS_CODE and <= 299;
    }
}
