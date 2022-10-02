namespace LazyTests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TestFunctions
{
    public static bool GuessNumber()
    {
        var random = new Random();
        var prediction = random.Next(2);
        var number = random.Next(2);
        return number == prediction;
    }

    public static int CalculateRandomValue()
    {
        var random = new Random();
        var firstArgument = random.Next(1000);
        var secondArgument = random.Next(1000);
        return firstArgument + secondArgument;
    }

    public static string СoncatenateString()
    {
        var random = new Random();
        var stringLength = random.Next(5);
        var newString = string.Empty;
        for (var i = 0; i < stringLength; ++i)
        {
            newString += "f";
        }

        return newString;
    }

    public static object CreateNewObject()
    {
        return new object();
    }

    public static object? ReturnNull()
    {
        return null;
    }
}
