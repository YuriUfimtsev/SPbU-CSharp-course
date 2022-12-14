using System.Net.Sockets;
using System.Net;

namespace NetworkChat;

public static class Server
{
    public static async Task Run(int port)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Ready to communicate");
        var socket = await listener.AcceptSocketAsync();
        Console.WriteLine("The connection is established");
        var stream = new NetworkStream(socket);
        var reader = new StreamReader(stream);
        var writer = new StreamWriter(stream);
        while (true)
        {
            var data = await reader.ReadLineAsync();
            if (data == "exit")
            {
                break;
            }

            Console.WriteLine($"received: {data}");

            var response = Console.ReadLine();
            await writer.WriteLineAsync(response);
            await writer.FlushAsync();
            Console.WriteLine($"Sent: {response}");
            if (response == "exit")
            {
                break;
            }
        }

        socket.Close();
    }
}
