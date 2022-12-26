namespace SimpleFTPTests;

using SimpleFTPClient;
using System.Net.Sockets;
using System.Net;
using static SimpleFTPClient.Client;

public class ClientTests
{
    private CancellationTokenSource cancellationTokenSource;

    private static string pathForTestListRequest = "../SimpleFTPTests/TestDirectory";
    private static string testListResponse = "2 ../SimpleFTPTests/TestDirectory/NestedFolder true" +
    " ../SimpleFTPTests/TestDirectory/Text.txt false\n";

    private static string pathForTestGetRequest = "../../../TestDirectory/TextFirst.txt";
    private static byte[] textFileBytes = File.ReadAllBytes("../../../TestDirectory/TextFirst.txt");
    private static string testGetResponse = $"{textFileBytes.Length} {System.Text.Encoding.Default.GetString(textFileBytes)}\n";

    private static string pathForUnusualTestGetRequest = "../../../TestDirectory/NestedFolder/Text1.txt";
    private static byte[] text1FileBytes = File.ReadAllBytes("../../../TestDirectory/NestedFolder/Text1.txt");
    private static string unusualTestGetResponse = $"{text1FileBytes.Length} {System.Text.Encoding.Default.GetString(text1FileBytes)}\n";

    private static string pathForIncorrectResponseTestListRequest = "../SimpleFTPTests/TestDirectory/NestedFolder11";

    [SetUp]
    public void Setup()
    {
        cancellationTokenSource = new CancellationTokenSource();
    }

    [Test]
    public async Task StandartListRequestTest()
    {
        var client = new Client(8888);
        Task.Run(async () => await ServerStartMoq(8888, cancellationTokenSource.Token));
        var result = await client.List(pathForTestListRequest);
        cancellationTokenSource.Cancel();
        var expectedResult = new List<DirectoryElement>
        {
            new DirectoryElement("../SimpleFTPTests/TestDirectory/NestedFolder", true),
            new DirectoryElement("../SimpleFTPTests/TestDirectory/Text.txt", false)
        };
        Assert.That(CompareDirectoryElementLists(result, expectedResult), Is.EqualTo(0));
    }

    [Test]
    public async Task StandartGetRequestTest()
    {
        var client = new Client(8887);
        Task.Run(async () => await ServerStartMoq(8887, cancellationTokenSource.Token));
        using var result = new MemoryStream();
        await client.Get(pathForTestGetRequest, result);
        cancellationTokenSource.Cancel();
        var expectedResult = textFileBytes;
        var arrayResult = result.ToArray();
        Array.Resize(ref arrayResult, expectedResult.Length);
        Assert.That(arrayResult, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task GetRequestTestWithEndOfLineCharacterInTheMiddle()
    {
        var client = new Client(8889);
        Task.Run(async () => await ServerStartMoq(8889, cancellationTokenSource.Token));
        using var result = new MemoryStream();
        await client.Get(pathForUnusualTestGetRequest, result);
        cancellationTokenSource.Cancel();
        var expectedResult = text1FileBytes;
        var arrayResult = result.ToArray();
        Assert.That(arrayResult, Is.EqualTo(expectedResult));
    }

    [Test]
    public void IvalidPathInServerResponseTest()
    {
        var client = new Client(8890);
        Task.Run(async () => await ServerStartMoq(8890, cancellationTokenSource.Token));
        using var result = new MemoryStream();
        Assert.ThrowsAsync<InvalidPathException>(() => client.List(pathForIncorrectResponseTestListRequest));
    }

    private async Task ServerStartMoq(int port, CancellationToken cancellationToken)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        while (!cancellationToken.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync();
            var task = Task.Run(async () =>
            {
                var stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var data = await reader.ReadLineAsync();
                var response = data == null ? "-1\n" : ServerGenerateResponseMoq(data);
                var writer = new StreamWriter(stream);
                await writer.WriteAsync(response);
                await writer.FlushAsync();
                socket.Close();
            });
        }
    }

    private static string ServerGenerateResponseMoq(string request)
    {
        if (request == $"1 {pathForTestListRequest}")
        {
            return testListResponse;
        }
        else if (request == $"2 {pathForTestGetRequest}")
        {
            return testGetResponse;
        }
        else if (request == $"2 {pathForUnusualTestGetRequest}")
        {
            return unusualTestGetResponse;
        }
        else
        {
            return "-1";
        }
    }

    private static int CompareDirectoryElementLists(
        List<DirectoryElement> firstCollection,
        List<DirectoryElement> secondCollection)
    {
        if (firstCollection.Count != secondCollection.Count)
        {
            return -1;
        }

        var comparasion = new Comparison<DirectoryElement>((firstDirectoryElement, secondDirectoryElement)
            => string.Compare(firstDirectoryElement.Name, secondDirectoryElement.Name));
        firstCollection.Sort(comparasion);
        secondCollection.Sort(comparasion);
        for (var i = 0; i < firstCollection.Count; ++i)
        {
            if (!CompareDirectoryElements(firstCollection[i], secondCollection[i]))
            {
                return -1;
            }
        }

        return 0;
    }
    private static bool CompareDirectoryElements(DirectoryElement firstDirectoryElement, DirectoryElement secondDirectoryElement)
        =>firstDirectoryElement.Name == secondDirectoryElement.Name
            && firstDirectoryElement.IsDirectory == secondDirectoryElement.IsDirectory;
}
