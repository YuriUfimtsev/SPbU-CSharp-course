namespace SimpleFTPServer;

using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server
{
    private int port;
    private CancellationToken cancellationToken;

    public Server(int port, CancellationToken cancellationToken)
    {
        this.port = port;
        this.cancellationToken = cancellationToken;
    }

    private static string GenerateResponseToList(string pathToDirectory)
    {
        var response = new StringBuilder();
        try
        {
            var subDirectories = Directory.GetDirectories(pathToDirectory);
            var files = Directory.GetFiles(pathToDirectory);
            response.Append((subDirectories.Length + files.Length).ToString());
            foreach (var directory in subDirectories)
            {
                response.Append(' ');
                response.Append($"{directory} true");
            }

            foreach (var file in files)
            {
                response.Append(' ');
                response.Append($"{file} false");
            }

            response.Append("\n");
            return response.ToString();
        }
        catch (DirectoryNotFoundException)
        {
            response.Clear();
            response.Append("-1\n");
            return response.ToString();
        }
    }

    private static string GenerateResponseToGet(string pathToFile)
    {
        var response = new StringBuilder();
        try
        {
            if (!Path.HasExtension(pathToFile))
            {
                response.Append("-1\n");
                return response.ToString();
            }

            var fileContent = File.ReadAllBytes(pathToFile);
            response.Append(fileContent.Length.ToString());
            response.Append(' ');
            response.AppendLine(Encoding.Default.GetString(fileContent));

            response.Append("\n");
            return response.ToString();
        }
        catch (DirectoryNotFoundException)
        {
            response.Clear();
            response.Append("-1\n");
            return response.ToString();
        }
    }

    public static string GenerateResponse(string requestData)
    {
        var result = requestData[0] switch
        {
            '1' => GenerateResponseToList(requestData.Remove(0, 2)),
            '2' => GenerateResponseToGet(requestData.Remove(0, 2)),
            _ => "Request number not found",
        };

        return result;
    }

    public async Task Start()
    {
        var listener = new TcpListener(IPAddress.Any, this.port);
        listener.Start();
        while (!this.cancellationToken.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync();
            var task = Task.Run(async () =>
            {
                var stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var data = await reader.ReadLineAsync();
                var response = data == null ? "-1\n" : GenerateResponse(data);
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(response);
                await writer.FlushAsync();
                socket.Close();
            });
        }
    }
}
