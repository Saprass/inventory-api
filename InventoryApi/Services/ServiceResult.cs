namespace InventoryApi.Services;

public enum StatusCode
{
    None = 0,
    Success = 200,
    Created = 201,
    NoContent = 204,
    NotModified = 304,
    BadRequest = 400,
    NotFound = 404, 
    Conflict = 409,
    Forbiden = 403
}

public readonly record struct ServiceResult<T>(StatusCode StatusCode, T? Value, string? Error)
{
    public static ServiceResult<T> Success(T value) => new(StatusCode.Success, value, null);
    public static ServiceResult<T> Created(T value) => new(StatusCode.Created, value, null);
    public static ServiceResult<T> BadRequest(string error) => new(StatusCode.BadRequest, default, error);

}

public readonly record struct ServiceResult(StatusCode StatusCode, string? Error)
{
    public static ServiceResult NoContent() => new(StatusCode.NoContent, null);
    public static ServiceResult BadRequest(string error) => new(StatusCode.BadRequest, error);
    public static ServiceResult NotFound(string error) => new(StatusCode.NotFound, error);
    public static ServiceResult Conflict(string error) => new(StatusCode.Conflict, error);
    public static ServiceResult NotModified(string error) => new(StatusCode.NotModified, error);
}
