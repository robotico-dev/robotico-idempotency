using System.Collections.Concurrent;
using Robotico.Idempotency;
using Robotico.Result.Errors;

namespace Robotico.Idempotency.Tests;

/// <summary>
/// In-memory implementation of <see cref="IIdempotencyStore"/> for tests.
/// </summary>
public sealed class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly ConcurrentDictionary<string, byte> _claimed = new();

    /// <inheritdoc />
    public Task<Robotico.Result.Result> TryClaimAsync(string key, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (string.IsNullOrWhiteSpace(key))
        {
            return Task.FromResult(Robotico.Result.Result.Error(new SimpleError("Idempotency key cannot be empty.")));
        }

        return Task.FromResult(_claimed.TryAdd(key, 0) ? Robotico.Result.Result.Success() : Robotico.Result.Result.Error(new SimpleError("Key already claimed.")));
    }
}
