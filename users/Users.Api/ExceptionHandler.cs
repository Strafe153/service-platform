using System.Net;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.Diagnostics;
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

		await Results
			.Problem(exception.Message, httpContext.Request.Path, (int)statusCode)
			.ExecuteAsync(httpContext);

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