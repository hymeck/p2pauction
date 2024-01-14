namespace P2PAuction;

public sealed class BidService
{
    private readonly BidRepository _bidRepository;

    public BidService(BidRepository bidRepository)
    {
        _bidRepository = bidRepository;
    }

    public async Task<bool> PlaceAsync(BidModel bid)
    {
        var auctionBids = await _bidRepository.GetByAuctionAsync(bid.AuctionId);

        if (auctionBids.Count != 0)
        {
            var maxBid = auctionBids.MaxBy(b => b.Amount)!;
            if (maxBid.Amount >= bid.Amount)
                return false;
        }
        await _bidRepository.AddAsync(bid);

        return true;
    }

    public async Task<BidModel> GetHighestAuctionBid(Guid auctionId)
    {
        var auctionBids = await _bidRepository.GetByAuctionAsync(auctionId);

        return auctionBids.MaxBy(b => b.Amount)!;
    }
}
