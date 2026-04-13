

using System.Xml.Linq;
using Microsoft.AspNetCore.Diagnostics;

public static class WebApplicationExtensions
{
    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler("/error");
        app.Map("/error", (HttpContext httpContext) =>
        {
            var exceptionHandlerFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

            var exception = exceptionHandlerFeature?.Error;
            return Results.Problem(
                detail: exception?.ToString(), // Development only !!!
                statusCode: 500,
                title: exception?.Message
            );
        });
        return app;
    }
}