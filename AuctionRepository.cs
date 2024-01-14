namespace P2PAuction;

public sealed class AuctionRepository
{
    private readonly Dictionary<Guid, AuctionModel> _auctions = new Dictionary<Guid, AuctionModel>();

    public async Task AddAsync(AuctionModel auction)
    {
        _auctions[auction.Id] = auction;

        await Task.CompletedTask;
    }

    public async Task<AuctionModel> GetByIdAsync(Guid id) => await Task.FromResult(_auctions[id]);
}
