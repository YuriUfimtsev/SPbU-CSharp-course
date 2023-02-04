using SimpleFTPServer;

var cancellationTokenSource = new CancellationTokenSource();
var server = new Server(8888, cancellationTokenSource.Token);
var serverRunner = Task.Run(() => server.Start());
while (Console.ReadKey().Key != ConsoleKey.Enter)
{
}

cancellationTokenSource.Cancel();
await serverRunner;