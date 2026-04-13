

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
            
            return Results.Problem("An unexpected error occurred.");
        });
        return app;
    }
}