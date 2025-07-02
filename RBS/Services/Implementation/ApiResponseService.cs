using RBS.CORE;

namespace RBS.Services.Implenetation;

public class ApiResponseService<T>
{
    public static ApiResponse<T> Response(T? data, string? message, int status)
    {
        var response = new ApiResponse<T>
        {
            Data = data,
            Message = $"error: {message}",
            Status = status,
        };
        
        return response;
    }
    
    public static ApiResponse<T> Response200(T? data)
    {
        var response = new ApiResponse<T>
        {
            Data = data,
            Message = null,
            Status = StatusCodes.Status200OK,
        };
        
        return response;
    }
    
}