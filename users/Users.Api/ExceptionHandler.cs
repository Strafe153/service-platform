using System.Net;
using System.Net.Mime;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Users.Api;

public class ExceptionHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		var statusCode = GetHttpStatusCode(exception);
		var statusCodeAsInt = (int)statusCode;

		httpContext.Response.ContentType = MediaTypeNames.Application.Json;
		httpContext.Response.StatusCode = statusCodeAsInt;

		ProblemDetails problemDetails = new()
		{
			Status = statusCodeAsInt,
			Detail = exception.Message,
			Instance = httpContext.Request.Path
		};

		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

		return true;
	}

	private static HttpStatusCode GetHttpStatusCode(Exception exception) =>
		exception switch
		{
			NullReferenceException => HttpStatusCode.NotFound,
			OperationCanceledException
                or DbUpdateException
                or KeycloakException
                or ArgumentException => HttpStatusCode.BadRequest,
			_ => HttpStatusCode.InternalServerError
		};
}