

using backend.Extensions;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

public class MyControllerBase : ControllerBase
{
    protected IActionResult HandleError<T>(ErrorOr<T> result, Func<T, IActionResult> onSuccess)
    {
        if (result.IsError)
        {
            var error = result.FirstError;
            return Problem(
                statusCode: error.ToStatusCode(),
                title: error.Code,
                detail: error.Description
            );
        }

        return onSuccess(result.Value);
    }
}
