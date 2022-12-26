namespace SimpleFTPTests;

using Moq;
using SimpleFTPClient;
using SimpleFTPServer;
using System.Net.Sockets;
using System.Net;

public class ClientTests
{
    private CancellationTokenSource cancellationTokenSource;

    private static string testListRequest = "1 ../SimpleFTPTests/TestDirectory\n";
    private static string testListResponse = "2 ../SimpleFTPTests/TestDirectory/NestedFolder true" +
        " ../SimpleFTPTests/TestDirectory/Text.txt false\n";
    private static string testGetRequest = "2 ../SimpleFTPTests/TestDirectory/Text.txt\n";
    private static string testGetResponse = "12 TextTextText\n";
    private static Dictionary<string, string> testDataList = new()
    {
        { testListRequest, testListResponse },
        { testGetRequest, testGetResponse }
    };

    [SetUp]
    public void Setup()
    {
        cancellationTokenSource = new CancellationTokenSource();
    }

    [Test]
    public void StandartListRequestTest()
    {
        var client = new Client(8887);
        var mock = new Mock<Server>();
        mock.Setup((server => Server.GenerateResponse()));

    }
}