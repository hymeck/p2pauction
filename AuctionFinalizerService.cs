namespace P2PAuction;

public sealed class AuctionFinalizerService
{
    private readonly BidService _bidService;
    private readonly AuctionRepository _auctionRepository;

    public AuctionFinalizerService(BidService bidService, AuctionRepository auctionRepository)
    {
        _bidService = bidService;
        _auctionRepository = auctionRepository;
    }


    public async Task<AuctionModel> FinalizeAsync(Guid auctionId)
    {
        var highestBid = await _bidService.GetHighestAuctionBid(auctionId);

        var auction = await _auctionRepository.GetByIdAsync(auctionId);
        auction.Finalize(highestBid.BidderNodeId, highestBid.Amount);
        await _auctionRepository.AddAsync(auction);

        return auction;
    }
}
