using System.Net.Sockets;

const int port = 8888;
using (var client = new TcpClient("localhost", port))
{
    Console.WriteLine($"Sending to port {port}...");
    var stream = client.GetStream();
    var writer = new StreamWriter(stream);
    writer.Write("Hello, world!111");
    writer.Write("Hello, world!");
    writer.Flush();
}