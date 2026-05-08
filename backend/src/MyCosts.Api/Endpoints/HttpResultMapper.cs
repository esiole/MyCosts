using System.Diagnostics;
using MyCosts.Application.Common;

namespace MyCosts.Api.Endpoints;

internal static class HttpResultMapper
{
    public static IResult ToHttpResult(AppError error) => error.Kind switch
    {
        ErrorKind.Validation => Results.ValidationProblem(
            error.ValidationErrors?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            ?? new Dictionary<string, string[]> { [""] = [error.Message] }),
        ErrorKind.NotFound => Results.Problem(title: error.Message, statusCode: StatusCodes.Status404NotFound),
        ErrorKind.Conflict => Results.Problem(title: error.Message, statusCode: StatusCodes.Status409Conflict),
        ErrorKind.Unauthorized => Results.Problem(
            title: "Invalid credentials.",
            statusCode: StatusCodes.Status401Unauthorized),
        _ => throw new UnreachableException($"Unhandled {nameof(ErrorKind)}: {error.Kind}"),
    };
}
