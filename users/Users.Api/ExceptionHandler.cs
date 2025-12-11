using System.Net;
using FluentValidation;
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
		var statusCode = (int)GetHttpStatusCode(exception);

		if (exception is ValidationException validationException)
        {
			await HandleValidationException(httpContext, validationException, statusCode);
            return true;
        }

		await Results
			.Problem(exception.Message, httpContext.Request.Path, statusCode)
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
                or ArgumentException
				or ValidationException => HttpStatusCode.BadRequest,
			_ => HttpStatusCode.InternalServerError
		};

	private static async Task HandleValidationException(
		HttpContext httpContext,
		ValidationException exception,
		int statusCode)
    {
        var errors = exception.Errors
			.Select(e => new
			{
				Property = e.PropertyName,
				Error = e.ErrorMessage,
				Severity = e.Severity.ToString()
			})
			.ToList();

		ProblemDetails problem = new()
		{
			Status = statusCode,
			Title = "Validation failed",
			Detail = "One or more validation errors occurred.",
			Instance = httpContext.Request.Path
		};

		problem.Extensions[nameof(errors)] = errors;

		await Results.Json(problem).ExecuteAsync(httpContext);
    }
}