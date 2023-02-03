using System.Net.Sockets;
using System.Net;

namespace NetworkChat;

public static class Server
{
    public static async Task Run(int port, TextReader userReader, TextWriter userWriter)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        await userWriter.WriteLineAsync("Ready to communicate");
        using var socket = await listener.AcceptSocketAsync();
        await userWriter.WriteLineAsync("The connection is established");
        using var stream = new NetworkStream(socket);
        using var clientReader = new StreamReader(stream);
        using var clientWriter = new StreamWriter(stream);
        while (true)
        {
            var data = await clientReader.ReadLineAsync();
            if (data == "exit")
            {
                break;
            }

            await userWriter.WriteLineAsync($"received: {data}");
            var response = userReader.ReadLine();
            await clientWriter.WriteLineAsync(response);
            await clientWriter.FlushAsync();
            await userWriter.WriteLineAsync($"Sent: {response}");
            if (response == "exit")
            {
                break;
            }
        }

        socket.Close();
    }
}
