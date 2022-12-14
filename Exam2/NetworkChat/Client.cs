using System.Net.Sockets;

namespace NetworkChat;

public static class Client
{
    public static async Task Run(int port, string IPaddress)
    {
        using (var tcpClient = new TcpClient(IPaddress, port))
        {
            var stream = tcpClient.GetStream();
            var writer = new StreamWriter(stream);
            var reader = new StreamReader(stream);
            while (true)
            {
                var message = Console.ReadLine();
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();

                if (message == "exit")
                {
                    break;
                }

                Console.WriteLine($"Sent: {message}");
                var data = await reader.ReadLineAsync();
                Console.WriteLine($"Received: {data}");
                if (data == "exit")
                {
                    break;
                }
            }
        }
    }
}
