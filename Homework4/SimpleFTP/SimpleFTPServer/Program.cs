using System.Net;
using System.Net.Sockets;
using System.Text;

const int port = 8888;
var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine("Ready to communicate");
while (true)
{
    var socket = await listener.AcceptSocketAsync();
    Console.WriteLine("The connection is established");
    var task = Task.Run(async () =>
    {
        var stream = new NetworkStream(socket);
        var reader = new StreamReader(stream);
        var data = await reader.ReadLineAsync();
        var response = data == null ? "-1" : GenerateResponse(data);
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

string GenerateResponseToGet(string pathToFile)
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
