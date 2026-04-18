

using System.Xml.Linq;
using Microsoft.AspNetCore.Diagnostics;

public static class WebApplicationExtensions
{
    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler("/error");
        app.Map("/error", (HttpContext httpContext, IWebHostEnvironment env) =>
        {
            var exceptionHandlerFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandlerFeature?.Error;

            if (env.IsDevelopment())
            {
                return Results.Problem(detail: exception?.ToString(), title: exception?.Message);
            }

            return Results.Problem(detail: "An unexpected error occurred.", statusCode: 500);
        });
        return app;
    }
}