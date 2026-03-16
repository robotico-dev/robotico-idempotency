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
}
