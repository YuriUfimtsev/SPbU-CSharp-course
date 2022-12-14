namespace NetworkChat;

public class Program
{
    public static async Task Main(string[] args)
    {
        switch (args.Length)
        {
            case 1:
                var serverPort = 0;
                var serverParsingResult = int.TryParse(args[0], out serverPort);
                if (!serverParsingResult)
                {
                    Console.WriteLine("Incorrect port number");
                    return;
                }

                await Server.Run(serverPort, Console.In, Console.Out);
                break;
            case 2:
                var clientPort = 0;
                var clientParsingResult = int.TryParse(args[1], out clientPort);
                if (!clientParsingResult)
                {
                    Console.WriteLine("Incorrect port number");
                    return;
                }

                await Client.Run(clientPort, args[0], Console.In, Console.Out);
                break;
            default:
                Console.WriteLine("Incorrect input");
                break;
        }
    }
}