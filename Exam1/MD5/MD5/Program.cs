using MD5;

var testPath = "C:/Users/Home/source/repos/SPbU-CSharp-course/Exam1/MD5";
var test1Path = "../../../..";

Console.WriteLine("Enter path to directory");
string? path = Console.ReadLine();
try
{
    var checkSum = MD5.CheckSum.ComputeCheckSum(path!);
    for (var i = 0; i < checkSum.Length; ++i)
    {
        Console.Write(checkSum[i]);
        Console.Write(' ');
    }
}
catch (Exception exception)
{
    Console.WriteLine(exception.Message);
}
