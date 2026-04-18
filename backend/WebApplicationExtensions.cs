

using System.Xml.Linq;
using Microsoft.AspNetCore.Diagnostics;

public static class WebApplicationExtensions
{
    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler("/error");
        app.Map("/error", (HttpContext httpContext, IWebHostEnvironment env) =>
        {
            var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (env.IsDevelopment())
                return Results.Problem(statusCode: 500, title: exception?.Message, detail: exception?.ToString());

            return Results.Problem(statusCode: 500, title: "internalError");
            
        });
        return app;
    }
}