using Robotico.Idempotency;
using Xunit;

namespace Robotico.Idempotency.Tests;

public sealed class IdempotencyStoreTests
{
    [Fact]
    public void IIdempotencyStore_contract_exists()
    {
        Assert.True(typeof(IIdempotencyStore).IsInterface);
    }

    [Fact]
    public async Task TryClaimAsync_first_call_succeeds()
    {
        InMemoryIdempotencyStore store = new();
        Robotico.Result.Result r = await store.TryClaimAsync("key1");
        Assert.True(r.IsSuccess());
    }

    [Fact]
    public async Task TryClaimAsync_second_call_same_key_fails()
    {
        InMemoryIdempotencyStore store = new();
        await store.TryClaimAsync("key2");
        Robotico.Result.Result r = await store.TryClaimAsync("key2");
        Assert.True(r.IsError(out _));
    }

    [Fact]
    public async Task TryClaimAsync_different_keys_both_succeed()
    {
        InMemoryIdempotencyStore store = new();
        Robotico.Result.Result r1 = await store.TryClaimAsync("a");
        Robotico.Result.Result r2 = await store.TryClaimAsync("b");
        Assert.True(r1.IsSuccess());
        Assert.True(r2.IsSuccess());
    }

    [Fact]
    public async Task TryClaimAsync_null_key_throws_or_fails()
    {
        InMemoryIdempotencyStore store = new();
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await store.TryClaimAsync(null!));
    }

    /// <summary>
    /// Empty or whitespace key returns failed Result (per API contract; implementations may reject).
    /// </summary>
    [Fact]
    public async Task TryClaimAsync_empty_key_returns_error()
    {
        InMemoryIdempotencyStore store = new();
        Robotico.Result.Result r = await store.TryClaimAsync("");
        Assert.True(r.IsError(out _));
    }

    [Fact]
    public async Task TryClaimAsync_whitespace_key_returns_error()
    {
        InMemoryIdempotencyStore store = new();
        Robotico.Result.Result r = await store.TryClaimAsync("   ");
        Assert.True(r.IsError(out _));
    }

    /// <summary>
    /// Law: claiming the same key twice — first succeeds, second fails (idempotency guarantee).
    /// </summary>
    [Fact]
    public async Task Idempotency_law_second_claim_same_key_fails()
    {
        InMemoryIdempotencyStore store = new();
        string key = "idem-key-law";
        Robotico.Result.Result first = await store.TryClaimAsync(key);
        Robotico.Result.Result second = await store.TryClaimAsync(key);
        Assert.True(first.IsSuccess());
        Assert.True(second.IsError(out _));
    }

    /// <summary>
    /// Law: different keys can each be claimed once (idempotency is per-key).
    /// </summary>
    [Theory]
    [InlineData("key-a", "key-b")]
    [InlineData("req-1", "req-2")]
    [InlineData("idem-1", "idem-2")]
    public async Task Idempotency_law_different_keys_both_claim_succeed(string key1, string key2)
    {
        InMemoryIdempotencyStore store = new();
        Robotico.Result.Result r1 = await store.TryClaimAsync(key1);
        Robotico.Result.Result r2 = await store.TryClaimAsync(key2);
        Assert.True(r1.IsSuccess());
        Assert.True(r2.IsSuccess());
    }
}
