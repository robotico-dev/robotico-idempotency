namespace Robotico.Idempotency;

/// <summary>
/// Store for idempotency keys. Ensures a command is processed at most once per key.
/// </summary>
/// <remarks>
/// Implementations must reject null or empty keys (e.g. throw <see cref="ArgumentNullException"/> or return a failed Result).
/// Combine with Robotico.Outbox for exactly-once delivery; see docs/design.adoc.
/// </remarks>
public interface IIdempotencyStore
{
    /// <summary>
    /// Tries to claim the idempotency key. Returns success if the key was not yet seen; failure if already processed.
    /// </summary>
    /// <param name="key">Idempotency key (e.g. from request header or message).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result: success when key is claimed, failure when key was already used.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null (implementations should throw or return a failed Result).</exception>
    Task<Robotico.Result.Result> TryClaimAsync(string key, CancellationToken cancellationToken = default);
}
