using System.Net.Sockets;
using System.Net;

const int port = 8888;
var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine($"Listening on port {port}...");
using (var socket = listener.AcceptSocket())
{
    var stream = new NetworkStream(socket);
    var streamReader = new StreamReader(stream);
    var data = streamReader.ReadToEnd();
    Console.WriteLine($"Received: {data}");
}

listener.Stop();