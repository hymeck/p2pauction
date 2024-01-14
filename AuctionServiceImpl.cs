using Grpc.Core;

namespace P2PAuction;

public sealed class AuctionServiceImpl : AuctionService.AuctionServiceBase
{
    private readonly AuctionRepository _auctionRepository;
    private readonly BidService _bidService;
    private readonly AuctionFinalizerService _auctionFinalizerService;


    public AuctionServiceImpl(AuctionRepository auctionRepository, BidService bidService, AuctionFinalizerService auctionFinalizerService)
    {
        _auctionRepository = auctionRepository;
        _bidService = bidService;
        _auctionFinalizerService = auctionFinalizerService;
    }

    public override async Task<InitiateAuctionResponse> InitiateAuction(InitiateAuctionRequest request, ServerCallContext context)
    {
        var auction = new AuctionModel
        {
            Id = Guid.Parse(request.AuctionId),
            Item = request.Item,
            Price = request.InitialPrice,
            InitiatorNodeId = request.InitiatorNodeId,
        };
        await _auctionRepository.AddAsync(auction);

        return new InitiateAuctionResponse() { Succeed = true };
    }

    public override async Task<PlaceBidResponse> PlaceBid(PlaceBidRequest request, ServerCallContext context)
    {
        var bid = new BidModel
        {
            Id = Guid.NewGuid(),
            AuctionId = Guid.Parse(request.AuctionId),
            Amount = request.Amount,
            BidderNodeId = request.BidderNodeId,
        };
        var added = await _bidService.PlaceAsync(bid);

        return new PlaceBidResponse() { Succeed = added };
    }

    public override async Task<FinalizeAuctionResponse> FinalizeAuction(FinalizeAuctionRequest request, ServerCallContext context)
    {
        var auctionId = Guid.Parse(request.AuctionId);
        var auction = await _auctionFinalizerService.FinalizeAsync(auctionId);

        return new FinalizeAuctionResponse()
        {
            CloserNodeId = auction.CloserNodeId.GetValueOrDefault(),
            TotalAmount = auction.TotalAmount.GetValueOrDefault()
        };
    }
}
