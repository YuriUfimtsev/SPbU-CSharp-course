namespace SimpleFTPTests;

using Moq;
using SimpleFTPClient;

public class ClientTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void StandartListRequestTest()
    {
        var client = new Client(8887);
        var mock = new Mock<>;
    }
}