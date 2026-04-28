namespace Shared.DTOs
{
    public class ApiErrorDto
    {
        public string Message { get; set; }

        public ApiErrorDto(string message)
        {
            Message = message;
        }
    }
}
