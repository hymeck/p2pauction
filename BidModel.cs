namespace P2PAuction;

public sealed class BidModel
{
    public Guid Id { get; set; }
    public double Amount { get; set; }
    public Guid AuctionId { get; set; }
    public int BidderNodeId { get; set; }
}
