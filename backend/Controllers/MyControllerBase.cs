

using backend.Extensions;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

public class MyControllerBase : ControllerBase
{
   protected IActionResult? HandleError<T>(ErrorOr<T> result)
{
    if (!result.IsError) return null;

    var error = result.FirstError;

    return Problem(
        statusCode: error.ToStatusCode(),
        title: error.Code,
        detail: error.Description
    );
}
}