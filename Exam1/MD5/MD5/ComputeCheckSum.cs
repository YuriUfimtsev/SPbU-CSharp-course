namespace MD5;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security;

public class CheckSum
{
    private string mainPath;
    private bool isDirectory;

    public CheckSum(string path)
    {
        var PointPosition = path.IndexOf('.');
        this.isDirectory = PointPosition == -1;
        this.mainPath = path;
    }

    public byte[]? ComputeCheckSum()
    {
        if (this.mainPath == null)
        {
            return null;
        }

        if (!this.isDirectory)
        {
            var result = ComputeFileHash(mainPath);
            return result;
        }

        var pathToFileList = new List<string>();
        var hashValues = new List<byte[]>();
        var subdirectories = Directory.GetDirectories(this.mainPath);
        if (subdirectories == null || subdirectories.Length == 0)
        {
            Console.WriteLine("No files and folders in this path.");
            return null;
        }

        foreach (var subdirectory in subdirectories)
        {
            foreach (var pathToFile in Directory.GetFiles(subdirectory))
            {
                pathToFileList.Add(pathToFile);
            }
        }

        pathToFileList.Sort();

        for (var i = 0; i < pathToFileList.Count; ++i)
        {
            var fileHash = ComputeFileHash(pathToFileList[i]);
            if (fileHash != null)
            {
                hashValues.Add(fileHash);
            }
        }

        var resultHash = ConcatenateByteArraysAndDirectoryPath(hashValues);
        return resultHash;
    }

    private byte[] ConcatenateByteArraysAndDirectoryPath(List<byte[]> byteArrays)
    {
        var directoryPathInBytes = Encoding.ASCII.GetBytes(this.mainPath);
        var resultArrayLength = directoryPathInBytes.Length;
        foreach (var byteArray in byteArrays)
        {
            resultArrayLength += byteArray.Length;
        }

        var resultByteArray = new byte[resultArrayLength];
        var offset = 0;
        Buffer.BlockCopy(directoryPathInBytes, 0, resultByteArray, offset, directoryPathInBytes.Length);
        offset += directoryPathInBytes.Length;
        for (var i = 0; i < byteArrays.Count; ++i)
        {
            Buffer.BlockCopy(byteArrays[i], 0, resultByteArray, offset, byteArrays[i].Length);
            offset += byteArrays[i].Length;
        }

        return resultByteArray;
    }

    private byte[]? ComputeFileHash(string pathToFile)
    {
        try
        {
            var result = File.ReadAllBytes(pathToFile);
            var fileHash = System.Security.Cryptography.MD5.HashData(result);
            return fileHash;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"File {pathToFile} hasn't been cached because '{exception}' Exception caught.");
            return null;
        }
    }
}