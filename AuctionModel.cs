namespace P2PAuction;

public sealed class AuctionModel
{
    public Guid Id { get; set; }
    public string Item { get; set; }
    public double Price { get; set; }
    public int InitiatorNodeId { get; set; }
    public double? TotalAmount { get; set; }
    public int? CloserNodeId { get; set; }

    public void Finalize(int closerNodeId, double priceSold)
    {
        CloserNodeId = closerNodeId;
        TotalAmount = priceSold;
    }
}
