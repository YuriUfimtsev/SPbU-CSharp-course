namespace SimpleFTPClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        const int port = 8888;
        if (args.Length == 0 || args.Length > 2)
        {
            Console.WriteLine( Directory.GetCurrentDirectory() +"Incorrect arguments");
            return;
        }

        var client = new Client(port);
        try
        {
            switch (args[0])
            {
                case "1":
                    var result = await client.List(args[1]);
                    break;
                case "2":
                    var resultFileName = "GetResult.txt";
                    using (var fileStream = new FileStream(resultFileName, FileMode.Create))
                    {
                        await client.Get(args[1], fileStream);
                        Console.WriteLine($"See the result in the file {resultFileName}");
                    }

                    break;
                default:
                    Console.WriteLine("Incorrect request number");
                    break;
            }
        }
        catch (InvalidPathException)
        {
            Console.WriteLine("Non-existent path in the request");
        }
        catch (InvalidServerResponseException)
        {
            Console.WriteLine("Server error");
        }
    }
}
