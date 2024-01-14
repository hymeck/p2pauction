using System.Collections.ObjectModel;

namespace P2PAuction;

public sealed class PeerNodeRepository
{
    private readonly Dictionary<string, PeerNodeModel> _knownNodes = new Dictionary<string, PeerNodeModel>();

    public async Task AddAsync(PeerNodeModel node)
    {
        _knownNodes[node.Address] = node;

        await Task.CompletedTask;
    }

    public async Task<ReadOnlyCollection<PeerNodeModel>> GetAllAsync() =>
        await Task.FromResult(_knownNodes.Values.ToList().AsReadOnly());
}
