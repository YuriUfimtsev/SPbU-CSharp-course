using System.Net.Sockets;
using System.Net;

const int port = 8888;
var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine($"Listening on port {port}...");
while (true)
{
var socket = await listener.AcceptSocketAsync();
var task = Task.Run(async () =>
    {
        var stream = new NetworkStream(socket);
        var reader = new StreamReader(stream);
        var data = await reader.ReadLineAsync();
        if (data == null)
        {
            return;
        }

        var response = GenerateResponse(data);

        Console.WriteLine($"Sending \"Hi!\"");
        var writer = new StreamWriter(stream);
        await writer.WriteAsync("Hi!");
        await writer.FlushAsync();
        socket.Close();
    });
}

string GenerateResponseToList(string pathToDirectory)
{

}

string GenerateResponseToGet(string pathToFile)
{

}

string GenerateResponse(string requestData)
{
    var result = requestData[0] switch
    {
        '1' => GenerateResponseToList(requestData.Remove(0, 2)),
        '2' => GenerateResponseToGet(requestData.Remove(0, 2)),
        _ => "Request number not found",
    };

    return result;
}
