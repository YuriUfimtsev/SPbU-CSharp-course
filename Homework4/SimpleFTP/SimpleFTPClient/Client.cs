namespace SimpleFTPClient;

using System;
using System.Net.Sockets;
using System.Text;

public class Client
{
    private int port;

    public Client(int port)
    {
        this.port = port;
    }

    public async Task<List<DirectoryElement>> List(string path)
    {
        using (var tcpClient = new TcpClient("localhost", this.port))
        {
            var stream = tcpClient.GetStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync($"1 {path}\n");
            await writer.FlushAsync();
            var reader = new StreamReader(stream);
            try
            {
                var data = await ReadListRequestData(reader);
                return data;
            }
            catch (Exception exception)
            {
                throw Equals(exception, typeof(ArgumentException)) ?
                    new InvalidPathException() : new InvalidServerResponseException();
            }
        }
    }

    public async Task Get(string pathToFile, Stream outputStream)
    {
        using (var tcpClient = new TcpClient("localhost", this.port))
        {
            var stream = tcpClient.GetStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync($"2 {pathToFile}\n");
            await writer.FlushAsync();
            var reader = new StreamReader(stream);
            try
            {
                var data = await ReadGetRequestData(reader);
                var outputWriter = new StreamWriter(outputStream);
                await outputWriter.WriteAsync(data);
                await outputWriter.FlushAsync();
            }
            catch (Exception exception)
            {
                throw Equals(exception, typeof(ArgumentException)) ?
                    new InvalidPathException() : new InvalidServerResponseException();
            }
        }
    }

    private static async Task<List<DirectoryElement>> ReadListRequestData(StreamReader streamReader)
    {
        var result = new List<DirectoryElement>();
        var data = await streamReader.ReadLineAsync();
        Console.WriteLine(data);
        if (data == null)
        {
            throw new InvalidDataException();
        }

        var dataArray = data.Split();
        var dataLength = int.Parse(dataArray[0]);
        if (dataLength == -1)
        {
            throw new ArgumentException();
        }

        for (var i = 1; i < dataLength; i += 2)
        {
            var boolValue = dataArray[i + 1] switch
            {
                "true" => true,
                "false" => false,
                _ => throw new InvalidDataException(),
            };
            var newElement = new DirectoryElement(dataArray[i], boolValue);
            result.Add(newElement);
        }

        return result;
    }

    private static async Task<char[]> ReadGetRequestData(StreamReader streamReader)
    {
        var currentSymbol = new char[1];
        await streamReader.ReadAsync(currentSymbol, 0, 1);
        var data = new StringBuilder();
        while (currentSymbol[0] != ' ' && currentSymbol[0] != '\n')
        {
            data.Append(currentSymbol);
            await streamReader.ReadAsync(currentSymbol, 0, 1);
        }

        var dataLength = int.Parse(data.ToString());
        if (dataLength == -1)
        {
            throw new ArgumentException();
        }

        var fileData = new char[dataLength];
        await streamReader.ReadAsync(fileData, 0, fileData.Length);
        return fileData;
    }

    public struct DirectoryElement
    {
        public DirectoryElement(string name, bool isDirectory)
        {
            this.Name = name;
            this.IsDirectory = isDirectory;
        }

        public string Name { get; }

        public bool IsDirectory { get; }
    }
}
