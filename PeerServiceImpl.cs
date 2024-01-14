using Grpc.Core;

namespace P2PAuction;

public sealed class PeerServiceImpl : PeerService.PeerServiceBase
{
    private readonly PeerNodeRepository _peerNodeRepository;

    public PeerServiceImpl(PeerNodeRepository peerNodeRepository)
    {
        _peerNodeRepository = peerNodeRepository;
    }

    public override async Task<RequestPeerAddResponse> RequestPeerAdd(RequestPeerAddRequest request, ServerCallContext context)
    {
        var knownNode = new PeerNodeModel
        {
            NodeId = request.NodeId,
            Address = request.Address
        };
        await _peerNodeRepository.AddAsync(knownNode);

        var response = new RequestPeerAddResponse() { Added = true };
        return await Task.FromResult(response);
    }
}
