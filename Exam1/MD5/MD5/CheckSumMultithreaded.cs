namespace MD5;

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public static class CheckSumConcurrent
{
    public static async Task<byte[]> ComputeCheckSum(string directory)
    {
        if (directory == null)
        {
            throw new InvalidDataException("Incorrect path.");
        }

        if (Path.HasExtension(directory))
        {
            try
            {
                var fileCheckSum = await ComputeFileHash(directory);
                return fileCheckSum;
            }
            catch (Exception exception)
            {
                throw new AggregateException($"File {directory} hasn't been cached because '{exception}' Exception caught.");
            }
        }

        var result = await ComputeHash(directory);
        return result;
    }

    private static async Task<byte[]> ComputeHash(string directory)
    {
        var hashValues = new List<byte[]>();
        var subdirectories = Directory.GetDirectories(directory);
        var filesInMainDirectory = Directory.GetFiles(directory);
        Array.Sort(subdirectories);
        Array.Sort(filesInMainDirectory);
        var computeFileHashProcesses = new Task<byte[]>[filesInMainDirectory.Length];
        var computeDirectoryHashProcesses = new Task<byte[]>[subdirectories.Length];///Или через TPL
        for (var i = 0; i < filesInMainDirectory.Length; ++i)
        {
            computeFileHashProcesses[i] = ComputeFileHash(filesInMainDirectory[i]);
        }

        for (var i = 0; i < subdirectories.Length; ++i)
        {
            computeDirectoryHashProcesses[i] = ComputeHash(subdirectories[i]);
        }

        for (var i = 0; i < filesInMainDirectory.Length; ++i)
        {
            try
            {
                var hash = await computeFileHashProcesses[i];
                if (hash != null)
                {
                    hashValues.Add(hash);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"File {filesInMainDirectory[i]} hasn't been cached because '{exception}' Exception caught.");
            }
        }

        for (var i = 0; i < subdirectories.Length; ++i)
        {
            var directoryHash = await computeDirectoryHashProcesses[i];
            hashValues.Add(directoryHash);
        }

        var result = ConcatenateByteArraysAndDirectoryPath(hashValues, directory);/// Не распараллелить же?
        return result;
    }

    private static byte[] ConcatenateByteArraysAndDirectoryPath(List<byte[]> byteArrays, string directoryPath)
    {
        var directoryPathInBytes = Encoding.ASCII.GetBytes(directoryPath);
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

    private static async Task<byte[]> ComputeFileHash(string pathToFile)
    {
        var result = File.ReadAllBytes(pathToFile);
        var fileHash = System.Security.Cryptography.MD5.HashData(result);
        return fileHash;
    }
}