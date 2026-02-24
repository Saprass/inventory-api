using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace InventoryApi.Services;

public static class ServiceResultExtensions
{
    public static IActionResult ToActionResult(this ServiceResult result, ControllerBase controller)
    {
        return result.Status switch
        {
            StatusCode.BadRequest => controller.BadRequest(result.Error),
            StatusCode.NotFound => controller.NotFound(result.Error),
            StatusCode.Conflict => controller.Conflict(result.Error),
            _ => controller.StatusCode((int)result.Status)
        };
    }

    public static IActionResult ToActionResult<T>(this ServiceResult<T> result, ControllerBase controller)
    {
        return result.Status switch
        {
            StatusCode.Success => controller.Ok(result.Value!),
            StatusCode.Created => controller.Created(result.Msg, result.Value),
            StatusCode.BadRequest => controller.BadRequest(result.Msg),
            StatusCode.NotFound => controller.NotFound(result.Msg),
            StatusCode.Conflict => controller.Conflict(result.Msg),
            _ => controller.StatusCode((int)result.Status)
        };
    }
}
