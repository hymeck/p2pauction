using Grpc.Net.Client;

namespace P2PAuction;

public sealed class AuctionNotificationService
{
    private readonly PeerNodeRepository _peerNodeRepository;

    public AuctionNotificationService(PeerNodeRepository peerNodeRepository)
    {
        _peerNodeRepository = peerNodeRepository;
    }

    public async Task BroadcastAuctionInitiatedAsync(AuctionModel auction)
    {
        var knownPeers = await _peerNodeRepository.GetAllAsync();
        foreach (var peer in knownPeers)
        {
            var channel = GrpcChannel.ForAddress(peer.Address);
            var client = new AuctionService.AuctionServiceClient(channel);
            var request = new InitiateAuctionRequest()
            {
                AuctionId = auction.Id.ToString(),
                Item = auction.Item,
                InitialPrice = auction.Price,
                InitiatorNodeId = auction.InitiatorNodeId,
            };

            await client.InitiateAuctionAsync(request);
        }
    }

    public async Task BroadcastBidPlacedAsync(BidModel bid)
    {
        var knownPeers = await _peerNodeRepository.GetAllAsync();
        foreach (var peer in knownPeers)
        {
            var channel = GrpcChannel.ForAddress(peer.Address);
            var client = new AuctionService.AuctionServiceClient(channel);
            var request = new PlaceBidRequest()
            {
                AuctionId = bid.AuctionId.ToString(),
                Amount = bid.Amount,
                BidderNodeId = bid.BidderNodeId,
            };

            await client.PlaceBidAsync(request);
        }
    }

    public async Task BroadcastAuctionFinalizedAsync(Guid auctionId)
    {
        var knownPeers = await _peerNodeRepository.GetAllAsync();
        foreach (var peer in knownPeers)
        {
            var channel = GrpcChannel.ForAddress(peer.Address);
            var client = new AuctionService.AuctionServiceClient(channel);
            var request = new FinalizeAuctionRequest()
            {
                AuctionId = auctionId.ToString()
            };

            await client.FinalizeAuctionAsync(request);
        }
    }
}
