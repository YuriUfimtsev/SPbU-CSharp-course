using System.Net.Sockets;

namespace NetworkChat;

public static class Client
{
    public static async Task Run(int port, string IPaddress, TextReader userReader, TextWriter userWriter)
    {
        using (var tcpClient = new TcpClient(IPaddress, port))
        {
            using var stream = tcpClient.GetStream();
            using var serverWriter = new StreamWriter(stream);
            using var serverReader = new StreamReader(stream);

            while (true)
            {
                var message = userReader.ReadLine();
                await serverWriter.WriteLineAsync(message);
                await serverWriter.FlushAsync();

                if (message == "exit")
                {
                    break;
                }

                await userWriter.WriteLineAsync($"Sent: {message}");
                var data = await serverReader.ReadLineAsync();
                await userWriter.WriteLineAsync($"Received: {data}");
                if (data == "exit")
                {
                    break;
                }
            }
        }
    }
}
