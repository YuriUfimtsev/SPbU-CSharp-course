namespace SimpleFTPServer;

using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Implements network server entity.
/// </summary>
public class Server
{
    private int port;
    private CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="port">Network connection port number.</param>
    /// <param name="cancellationToken">Server cancellatin token to suspend work.</param>
    public Server(int port, CancellationToken cancellationToken)
    {
        this.port = port;
        this.cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Starts the server.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Start()
    {
        var listener = new TcpListener(IPAddress.Any, this.port);
        listener.Start();
        var serverTasks = new List<Task>();
        while (!this.cancellationToken.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync(this.cancellationToken);
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
            serverTasks.Add(task);
        }

        await Task.WhenAll(serverTasks.ToArray());
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

    private static string GenerateResponse(string requestData)
    {
        var result = requestData[0] switch
        {
            '1' => GenerateResponseToList(requestData.Remove(0, 2)),
            '2' => GenerateResponseToGet(requestData.Remove(0, 2)),
            _ => "Request number not found",
        };

        return result;
    }
}
