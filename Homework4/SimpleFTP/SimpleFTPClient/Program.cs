using System.Runtime.Intrinsics.X86;

namespace SimpleFTPClient;

public class Program
{
    public static async void Main(string[] args)
    {
        {
            if (args.Length == 0 || args.Length > 2)
            {
                Console.WriteLine("Incorrect arguments");
            }

            var client = new Client(8888);
            switch (args[0])
            {
                case "1":
                    try
                    {
                        var result = await client.List(args[1]);
                    }
                    catch (AggregateException exception)
                    {
                        Console.WriteLine(exception.Message);
                        return;
                    }

                    break;
                case "2":
                    try
                    {
                        var resultFileName = "GetResult.txt";
                        using var fileStream = new FileStream(resultFileName, FileMode.Create);
                        client.Get(args[1], fileStream);
                        Console.WriteLine($"See the result in the file {resultFileName}");
                    }
                    catch (AggregateException exception)
                    {
                        Console.WriteLine(exception.Message);
                        return;
                    }

                    break;
                default:
                    Console.WriteLine("Incorrect request number");
                    break;
            }
        }
    }
}
