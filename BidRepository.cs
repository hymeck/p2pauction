using System.Collections.ObjectModel;

namespace P2PAuction;

public sealed class BidRepository
{
    private Dictionary<Guid, BidModel> _bids = new Dictionary<Guid, BidModel>();

    public async Task AddAsync(BidModel model)
    {
        _bids[model.Id] = model;

        await Task.CompletedTask;
    }

    public async Task<ReadOnlyCollection<BidModel>> GetByAuctionAsync(Guid auctionId)
    {
        var auctionBids = _bids.Values.Where(b => b.AuctionId == auctionId).ToList().AsReadOnly();

        return await Task.FromResult(auctionBids);
    }
}
