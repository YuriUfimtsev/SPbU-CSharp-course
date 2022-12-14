using System.Net.Sockets;

namespace NetworkChat;

public static class Client
{
    public static async Task Run(int port, string IPaddress, TextReader userReader, TextWriter userWriter)
    {
        using (var tcpClient = new TcpClient(IPaddress, port))
        {
            var stream = tcpClient.GetStream();
            var serverWriter = new StreamWriter(stream);
            var serverReader = new StreamReader(stream);

            while (true)
            {
                var message = userReader.ReadLine();
                await serverWriter.WriteLineAsync(message);
                await serverWriter.FlushAsync();

                if (message == "exit")
                {
                    break;
                }

                userWriter.WriteLine($"Sent: {message}");
                var data = await serverReader.ReadLineAsync();
                userWriter.WriteLine($"Received: {data}");
                if (data == "exit")
                {
                    break;
                }
            }
        }
    }
}
