namespace P2PAuction;

public sealed class GuidGeneratorService
{
    public Guid GenerateNew() => Guid.NewGuid();
}
