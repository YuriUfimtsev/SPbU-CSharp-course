namespace MyNUnit;

/// <summary>
/// Implements the entry point to the program.
/// </summary>
public class Program
{
    /// <summary>
    /// Entry point to the program.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("You should only enter the path.");
            return;
        }

        try
        {
            var information = TestRunner.RunTests(args[0]);
            TestRunner.GenerateTestRunnerReport(information, Console.Out);
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Incorrect path.");
        }
    }
}