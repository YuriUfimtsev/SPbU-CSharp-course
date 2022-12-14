using System.Net.Sockets;
using System.Net;

namespace NetworkChat;

public static class Server
{
    public static async Task Run(int port, TextReader userReader, TextWriter userWriter)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Ready to communicate");
        var socket = await listener.AcceptSocketAsync();
        Console.WriteLine("The connection is established");
        var stream = new NetworkStream(socket);
        var clientReader = new StreamReader(stream);
        var clientWriter = new StreamWriter(stream);
        while (true)
        {
            var data = await clientReader.ReadLineAsync();
            if (data == "exit")
            {
                break;
            }

            userWriter.WriteLine($"received: {data}");
            var response = userReader.ReadLine();
            await clientWriter.WriteLineAsync(response);
            await clientWriter.FlushAsync();
            userWriter.WriteLine($"Sent: {response}");
            if (response == "exit")
            {
                break;
            }
        }

        socket.Close();
    }
}
