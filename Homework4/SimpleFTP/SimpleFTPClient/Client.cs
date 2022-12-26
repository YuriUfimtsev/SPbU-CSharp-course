namespace SimpleFTPClient;

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Implements network client entity.
/// </summary>
public class Client
{
    private int port;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="port">Network connection port number.</param>
    public Client(int port)
    {
        this.port = port;
    }

    /// <summary>
    /// Initiates a List request.
    /// </summary>
    /// <param name="path">The path to the directory where List request will be executed by the server.</param>
    /// <returns>Collection of DirectoryElements in the requested catalog.</returns>
    public async Task<List<DirectoryElement>> List(string path)
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("localhost", this.port);
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
            throw Equals(exception.GetType(), typeof(ArgumentException)) ?
                new InvalidPathException() : new InvalidServerResponseException();
        }
    }

    /// <summary>
    /// Initiates a Get request.
    /// </summary>
    /// <param name="pathToFile">The path to the file that will be processed by the server.</param>
    /// <param name="outputStream">The stream with file data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Get(string pathToFile, Stream outputStream)
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("localhost", this.port);
        var stream = tcpClient.GetStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync($"2 {pathToFile}\n");
        await writer.FlushAsync();
        var reader = new StreamReader(stream);
        try
        {
            await ReadGetRequestData(stream, outputStream);
        }
        catch (Exception exception)
        {
            throw Equals(exception.GetType(), typeof(ArgumentException)) ?
                new InvalidPathException() : new InvalidServerResponseException();
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

        for (var i = 1; i < dataLength * 2; i += 2)
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

    private static async Task ReadGetRequestData(Stream sourceStream, Stream outputStream)
    {
        var currentSymbol = new char[1];
        var streamReader = new StreamReader(sourceStream);
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

        await sourceStream.CopyToAsync(outputStream, dataLength);
    }

    /// <summary>
    /// Implements file or catalog entity.
    /// </summary>
    public struct DirectoryElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryElement"/> struct.
        /// </summary>
        /// <param name="name">Catalog element name.</param>
        /// <param name="isDirectory">Shows it is the directory or the file.</param>
        public DirectoryElement(string name, bool isDirectory)
        {
            this.Name = name;
            this.IsDirectory = isDirectory;
        }

        /// <summary>
        /// Gets the directory element name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether it is a directory.
        /// </summary>
        public bool IsDirectory { get; }
    }
}
