namespace NetworkChatTests;

public class NetworkChatTests
{
    [Test]
    public void StandartTest()
    {
        var serverInput = File.OpenRead("ServerInput.txt");
        var clientInput = File.OpenRead("ClientInput.txt");
        var output = File.OpenWrite("Output.txt");
        Task.Run(() => NetworkChat.Server.Run(8082, new StreamReader(serverInput) as TextReader, new StreamWriter(output) as TextWriter));
        var result = NetworkChat.Client.Run(8082, "localhost", new StreamReader(clientInput) as TextReader, new StreamWriter(output) as TextWriter);
        result.Wait();
    }
}