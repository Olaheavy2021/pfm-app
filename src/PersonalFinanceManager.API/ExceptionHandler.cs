using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;

namespace PersonalFinanceManager.API;

public static class ExceptionHandler
{
    public static void UseCommonExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var logger = context
                    .RequestServices.GetRequiredService<Serilog.ILogger>()
                    .ForContext(
                        "RequestPath",
                        context.Request.Path == PathString.Empty
                            ? context.Request.PathBase.ToString()
                            : context.Request.Path.ToString()
                    );
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = MediaTypeNames.Text.Plain;

                var title = "An error occurred while processing your request.";
                var detail = default(string);
                var type = "https://tools.ietf.org/html/rfc9110#section-15.6.1";

                if (
                    context.RequestServices.GetService<IProblemDetailsService>() is
                    { } problemDetailsService
                )
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    var exceptionType = exceptionHandlerFeature?.Error;
                    if (exceptionType != null)
                    {
                        if (exceptionType is BadHttpRequestException badRequest)
                        {
                            context.Response.StatusCode = badRequest.StatusCode;
                            title = "Bad Request";
                            type =
                                "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request";
                            var builder = new StringBuilder();
                            Exception? exc = badRequest;
                            while (exc != null)
                            {
                                builder.AppendLine(exc.Message);
                                exc = exc.InnerException;
                            }
                            detail = builder.ToString();
                        }
                        //else if (app.Environment.IsDevelopment() && exceptionType is Riok.Mapperly.Abstractions.exc)
                        //{
                        //    title = "Missing mapping";
                        //    var builder = new StringBuilder();
                        //    Exception? exc = mappingException;
                        //    while (exc != null)
                        //    {
                        //        builder.AppendLine(exc.Message);
                        //        exc = exc.InnerException;
                        //    }
                        //    detail = builder.ToString();
                        //}

                        logger.Error(exceptionType, title);
                    }
                    else
                    {
                        logger.Error(title);
                    }

                    await problemDetailsService.WriteAsync(
                        new ProblemDetailsContext
                        {
                            HttpContext = context,
                            ProblemDetails =
                            {
                                Title = title,
                                Detail = detail,
                                Type = type,
                            },
                        }
                    );
                }
                else
                {
                    logger.Error(title);
                }
            });
        });
    }
}
