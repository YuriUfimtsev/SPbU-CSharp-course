using MD5;
using System;

var testPath = "C:/Users/Home/source/repos/SPbU-CSharp-course/Exam1/MD5";
var test1Path = "../../../..";

var newCheckSum = new CheckSum(test1Path);
var checkSum = newCheckSum.ComputeCheckSum();
if (checkSum == null)
{
    Console.WriteLine("0");
    return;
}

for (var i = 0; i < checkSum.Length; ++i)
{
    Console.Write(checkSum[i]);
    Console.Write(' ');
}
