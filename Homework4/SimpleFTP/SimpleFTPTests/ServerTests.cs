using SimpleFTPServer;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace SimpleFTPTests;

public class ServerTests
{
    private CancellationTokenSource cancellationTokenSource;

    private static string testGetRequest = "2 ../../../TestDirectory/TextFirst.txt";
    private static byte[] textFileBytes = File.ReadAllBytes("../../../TestDirectory/TextFirst.txt");
    private static string testGetExpectedResponse = $"{System.Text.Encoding.Default.GetString(textFileBytes)}\n";

    private static string incorrectTestListRequest = "1 ../../../TestDirectory/Text1.txt";
    private static string expectedIncorrectTestListResponse = "-1";

    [SetUp]
    public void Setup()
    {
        cancellationTokenSource = new CancellationTokenSource();
    }

    [Test]
    public async Task StandartGetResponseTest()
    {
        var server = new Server(8118, cancellationTokenSource.Token);
        Task.Run(async () => await server.Start());
        var result = await ClientMoq(8118, testGetRequest);
        cancellationTokenSource.Cancel();
        Assert.That(result.Remove(result.Length - 2) == testGetExpectedResponse.Remove(testGetExpectedResponse.Length - 1));
    }

    [Test]
    public async Task IncorrectPathRequestToServerTest()
    {
        var server = new Server(8113, cancellationTokenSource.Token);
        Task.Run(async () => await server.Start());
        var result = await ClientMoq(8113, incorrectTestListRequest);
        cancellationTokenSource.Cancel();
        Assert.That(result == expectedIncorrectTestListResponse);
    }

    [Test]
    public void MultipleClientsTest()
    {
        var server = new Server(8114, cancellationTokenSource.Token);
        Task.Run(async () => await server.Start());
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        Task.WaitAll(
            ClientMoqWithFiveSecondsWaiting(8114),
            ClientMoqWithFiveSecondsWaiting(8114),
            ClientMoqWithFiveSecondsWaiting(8114),
            ClientMoqWithFiveSecondsWaiting(8114),
            ClientMoqWithFiveSecondsWaiting(8114));
        stopWatch.Stop();
        cancellationTokenSource.Cancel();
        Assert.That(stopWatch.ElapsedMilliseconds, Is.LessThan(25000));
    }

    private static async Task<string> ClientMoq(int port, string request)
    {
        using var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync("localhost", port);
        var stream = tcpClient.GetStream();
        var writer = new StreamWriter(stream);
        await writer.WriteAsync($"{request}\n");
        await writer.FlushAsync();
        var reader = new StreamReader(stream);
        var data = string.Empty;
        if (request[0] == '1')
        {
            data = await reader.ReadLineAsync();
        }
        else
        {
            data = await ReadGetResponse(reader);
        }

        return data!;
    }

    private static async Task<string> ReadGetResponse(StreamReader streamReader)
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
        return new string(fileData);
    }

    private async Task<string> ClientMoqWithFiveSecondsWaiting(int port)
    {
        var request = "1 ../../../TestDirectory/NestedFolder";
        await Task.Delay(5000);
        return await ClientMoq(port, request);
    }
}
