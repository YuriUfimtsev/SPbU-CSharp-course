using SimpleFTPServer;

var cancellationTokenSource = new CancellationTokenSource();
var server = new Server(8888, cancellationTokenSource.Token);
Task.Run(() => server.Start());
if (Console.ReadKey().Key == ConsoleKey.Enter)
{
    cancellationTokenSource.Cancel();
}