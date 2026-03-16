namespace Robotico.Idempotency;

/// <summary>
/// Store for idempotency keys. Ensures a command is processed at most once per key.
/// </summary>
public interface IIdempotencyStore
{
    /// <summary>
    /// Tries to claim the idempotency key. Returns success if the key was not yet seen; failure if already processed.
    /// </summary>
    /// <param name="key">Idempotency key (e.g. from request header or message).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result: success when key is claimed, failure when key was already used.</returns>
    Task<Robotico.Result.Result> TryClaimAsync(string key, CancellationToken cancellationToken = default);
}
