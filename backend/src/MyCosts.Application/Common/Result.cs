using System.ComponentModel.DataAnnotations;

namespace MyCosts.Application.Common;

public enum ErrorKind
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized
}

public readonly record struct AppError(
    ErrorKind Kind,
    string Message,
    IReadOnlyDictionary<string, string[]>? ValidationErrors);

public sealed class Result<T>
{
    private readonly T? _value;
    private readonly AppError _error;

    public bool IsSuccess { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value on a failed Result.");

    public AppError Error => !IsSuccess
        ? _error
        : throw new InvalidOperationException("Cannot access Error on a successful Result.");

    private Result(T value)
    {
        _value = value;
        IsSuccess = true;
    }

    private Result(AppError error)
    {
        _error = error;
        IsSuccess = false;
    }

    public static Result<T> Ok(T value) => new(value);
    public static Result<T> Fail(AppError error) => new(error);

    public static Result<T> Fail(ErrorKind kind, string message) =>
        new(new AppError(kind, message, null));

    public static Result<T> ValidationFail(IEnumerable<ValidationResult> errors)
    {
        var dict = errors
            .SelectMany(e => e.MemberNames.Any()
                ? e.MemberNames.Select(m => (Field: m, Message: e.ErrorMessage ?? string.Empty))
                : [(Field: string.Empty, Message: e.ErrorMessage ?? string.Empty)])
            .GroupBy(x => x.Field, x => x.Message)
            .ToDictionary(g => g.Key, g => g.ToArray());

        return Fail(new AppError(ErrorKind.Validation, "One or more validation errors occurred.", dict));
    }

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<AppError, TOut> onFailure)
        => IsSuccess ? onSuccess(_value!) : onFailure(_error);
}
