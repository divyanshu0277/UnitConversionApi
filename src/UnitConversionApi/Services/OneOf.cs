namespace UnitConversionApi.Services;

/// <summary>
/// Lightweight discriminated union that holds either a TSuccess or a TError value.
/// This avoids taking a dependency on the OneOf NuGet package for a single use-case.
/// </summary>
public readonly struct OneOf<TSuccess, TError>
{
    private readonly TSuccess? _success;
    private readonly TError? _error;
    private readonly bool _isSuccess;

    private OneOf(TSuccess success)
    {
        _success = success;
        _isSuccess = true;
    }

    private OneOf(TError error)
    {
        _error = error;
        _isSuccess = false;
    }

    public static implicit operator OneOf<TSuccess, TError>(TSuccess value) => new(value);
    public static implicit operator OneOf<TSuccess, TError>(TError value) => new(value);

    public bool IsSuccess => _isSuccess;

    public TSuccess AsSuccess() => _isSuccess
        ? _success!
        : throw new InvalidOperationException("Result is an error.");

    public TError AsError() => !_isSuccess
        ? _error!
        : throw new InvalidOperationException("Result is a success.");

    public TResult Match<TResult>(Func<TSuccess, TResult> onSuccess, Func<TError, TResult> onError)
        => _isSuccess ? onSuccess(_success!) : onError(_error!);
}
