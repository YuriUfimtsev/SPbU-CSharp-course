using System.Net;
using System.Net.Sockets;
using System.Text;

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

        Console.WriteLine($"Sending \"{response}\"");
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(response);
        await writer.FlushAsync();
        socket.Close();
    });
}

string GenerateResponseToList(string pathToDirectory)
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
    }
    catch (DirectoryNotFoundException)
    {
        response.Clear();
        response.Append("-1");
        return response.ToString();
    }

    return response.ToString();
}

string GenerateResponseToGet(string pathToFile)
{
    var response = new StringBuilder();
    try
    {
        var files = Directory.GetFiles(pathToFile);
        if (files.Length < 1 || files.Length > 1)
        {
            response.Append("-1");
            return response.ToString();
        }

        var fileContent = File.ReadAllBytes(files[0]);
        response.Append(fileContent.Length.ToString());
        response.Append(' ');
        response.AppendLine(Encoding.Default.GetString(fileContent));
    }
    catch (DirectoryNotFoundException)
    {
        response.Clear();
        response.Append("-1");
        return response.ToString();
    }

    return response.ToString();
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
