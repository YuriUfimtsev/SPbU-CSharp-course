using SimpleFTPServer;

var cancellationTokenSource = new CancellationTokenSource();
var server = new Server(8888, cancellationTokenSource.Token);
await server.Start();