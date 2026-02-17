namespace InventoryApi.Services;

public static class ResultHttpExtensions
{
    public static IResult ToHttpAsync(ServiceResult result) =>
        result.Status switch {
            StatusCode.BadRequest => Results.BadRequest(result.Error),
            StatusCode.NotFound => Results.NotFound(result.Error),
            StatusCode.Conflict => Results.Conflict(result.Error),
            _ => Results.StatusCode((int)result.Status)
        };

    public static async Task<IResult> ToHttpAsync<T>(ServiceResult<T> result, Func<T, Task<IResult>> onSuccess) =>
        result.Ok
        ? await onSuccess(result.Value!)
        : result.Status switch {
            StatusCode.BadRequest => Results.BadRequest(result.Error),
            StatusCode.NotFound => Results.NotFound(result.Error),
            StatusCode.Conflict => Results.Conflict(result.Error),
            _ => Results.StatusCode((int)result.Status)
        };
}
