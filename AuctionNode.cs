using Grpc.Core;
using Grpc.Net.Client;

namespace P2PAuction;

public sealed class AuctionNode
{
    private readonly Server _grpcServer;
    private readonly AuctionNotificationService _auctionNotificationService;
    private readonly AuctionRepository _auctionRepository;
    private readonly PeerNodeRepository _peerNodeRepository;
    private readonly BidService _bidService;
    private readonly GuidGeneratorService _guidGeneratorService;
    private readonly AuctionFinalizerService _auctionFinalizerService;
    public int NodeId { get; }
    public string Address { get; }

    public AuctionNode(int nodeId, int port)
    {
        NodeId = nodeId;
        Address = "http://localhost:" + port;
        _peerNodeRepository = new PeerNodeRepository();
        _auctionNotificationService = new AuctionNotificationService(_peerNodeRepository);
        _auctionRepository = new AuctionRepository();
        _bidService = new BidService(new BidRepository());
        _guidGeneratorService = new GuidGeneratorService();
        _auctionFinalizerService = new AuctionFinalizerService(_bidService, _auctionRepository);
        _grpcServer = new Server()
        {
            Services = 
            { 
                PeerService.BindService(new PeerServiceImpl(_peerNodeRepository)), 
                AuctionService.BindService(new AuctionServiceImpl(_auctionRepository, _bidService, _auctionFinalizerService)) 
            },
            Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) },
        };
    }

    public async Task StartAsync()
    {
        _grpcServer.Start();
        await Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        await _grpcServer.ShutdownAsync();
    }

    public async Task<bool> RequestPeerAddAsync(string address)
    {
        var channel = GrpcChannel.ForAddress(address);
        var client = new PeerService.PeerServiceClient(channel);
        var request = new RequestPeerAddRequest() { NodeId = NodeId, Address = Address };
        var response = await client.RequestPeerAddAsync(request);
        return response.Added;
    }

    public async Task<Guid> InitiateAuctionAsync(string item, double initialPrice)
    {
        var auctionId = _guidGeneratorService.GenerateNew();
        var auction = new AuctionModel
        {
            Id = auctionId,
            Item = item,
            Price = initialPrice,
            InitiatorNodeId = NodeId
        };
        await _auctionRepository.AddAsync(auction);
        Console.WriteLine($"Client#{NodeId} opens auction: sell {item} for {initialPrice} USDT. Auction ID: {auctionId}");
        await _auctionNotificationService.BroadcastAuctionInitiatedAsync(auction);
        return auctionId;
    }

    public async Task<bool> PlaceBidAsync(Guid auctionId, double amount)
    {
        var bidId = _guidGeneratorService.GenerateNew();
        var bid = new BidModel
        {
            Id = bidId,
            Amount = amount,
            BidderNodeId = NodeId,
            AuctionId = auctionId
        };
        var placed = await _bidService.PlaceAsync(bid);
        Console.WriteLine($"Client#{NodeId} bids {amount} USDT for auction {auctionId}.");
        await _auctionNotificationService.BroadcastBidPlacedAsync(bid);
        return placed;
    }

    public async Task FinalizeAuctionAsync(Guid auctionId)
    {
        var auction = await _auctionFinalizerService.FinalizeAsync(auctionId);
        Console.WriteLine($"Client#{NodeId} finalized auction {auction.Id}, informing all about sale to Client#{auction.CloserNodeId.GetValueOrDefault()} at {auction.TotalAmount.GetValueOrDefault()} USDT.");

        await _auctionNotificationService.BroadcastAuctionFinalizedAsync(auction.Id);
    }
}
