namespace NetworkChatTests;

public class NetworkChatTests
{
    [Test]
    public void StandartClientTest()
    {
        var serverInput = File.OpenRead("ServerInput.txt");
        var serverOutput = File.OpenRead("ServerOutput.txt");
        var clientInput = File.OpenRead("ClientInput.txt");
        var clientOutput = File.OpenRead("ClientOutput.txt");
        Task.Run(() => NetworkChat.Server.Run(8082, new StreamReader(serverInput) as TextReader, new StreamWriter(serverInput) as TextWriter));
        Task.Run(() => NetworkChat.Client.Run(8082, "localhost", new StreamReader(clientInput) as TextReader, new StreamWriter(clientOutput) as TextWriter));
    }

    [Test]
    public void StandartServerTest()
    {
        Assert.Pass();
    }
}