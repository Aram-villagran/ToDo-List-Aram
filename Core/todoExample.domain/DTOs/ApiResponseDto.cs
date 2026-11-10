namespace todoExample.domain.DTOs;

public class ApiResponseDto<T>
{
    public bool Error { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}
