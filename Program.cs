using P2PAuction;

//var node = new AuctionNode("Node1", 50051);
//await node.StartAsync();
//Console.WriteLine("Press any key to stop the node...");
//Console.ReadKey();

//await node.StopAsync();

var node1 = new AuctionNode(1, 50051);
var node2 = new AuctionNode(2, 50052);
var node3 = new AuctionNode(3, 50053);

await node1.StartAsync();
await node2.StartAsync();
await node3.StartAsync();

// ---- 1. add peers ---

await node1.RequestPeerAddAsync(node2.Address);
await node1.RequestPeerAddAsync(node3.Address);

await node2.RequestPeerAddAsync(node1.Address);
await node2.RequestPeerAddAsync(node3.Address);

await node3.RequestPeerAddAsync(node1.Address);
await node3.RequestPeerAddAsync(node2.Address);

// ---- 1. add peers ---


// ---- 2. open auctions ---

var auctionId1 = await node1.InitiateAuctionAsync("Pic1", 75);
var auctionId2 = await node2.InitiateAuctionAsync("Pic2", 60);

// ---- 2. open auctions ---

// ---- 3. place bids ---

await node2.PlaceBidAsync(auctionId1, 75);
await node3.PlaceBidAsync(auctionId1, 75.5);
await node2.PlaceBidAsync(auctionId1, 80); // counters

await node1.FinalizeAuctionAsync(auctionId1);

Console.ReadKey();

await node1.StopAsync();
await node2.StopAsync();
await node3.StopAsync();