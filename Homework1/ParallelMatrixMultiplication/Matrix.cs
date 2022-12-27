namespace ParallelMatrixMultiplication;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Implements "the matrix" object and operations on it, such as two types of multiplication.
/// </summary>
public class Matrix
{
    private static Random random = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    /// <param name="matrixElement">matrix as an array of arrays.</param>
    /// <param name="size">tuple of the number of rows and columns of the matrix.</param>
    /// <exception cref="InvalidDataException">throws this exception if at least one row or column
    /// doesn't match to size data.</exception>
    public Matrix((int Rows, int Columns) size, int[][] matrixElement)
    {
        if (matrixElement.Length != size.Rows)
        {
            throw new InvalidDataException("Incorrect matrix");
        }

        for (var i = 0; i < matrixElement.Length; ++i)
        {
            if (matrixElement[i].Length != size.Columns)
            {
                throw new InvalidDataException("Incorrect matrix");
            }
        }

        this.Size = size;
        this.TheMatrix = matrixElement;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    /// <param name="fileName">name of the file to select the matrix from.</param>
    /// <exception cref="InvalidDataException">throws this exception if matrix contains less than rowsCount*columnsCount elements.</exception>
    public Matrix(string fileName)
    {
        var rows = new List<int[]>();
        using StreamReader fileStreamForCountMatrixSize = new(fileName);
        var line = string.Empty;
        var columnsCount = 0;
        var rowsCount = 0;
        while ((line = fileStreamForCountMatrixSize.ReadLine()) != null)
        {
            try
            {
                int[] stringNumbers = line.Split().Select(int.Parse).ToArray();
                if (columnsCount > 0 && columnsCount != stringNumbers.Length)
                {
                    throw new InvalidDataException("Incorrect matrix");
                }

                columnsCount = stringNumbers.Length;
                ++rowsCount;
                rows.Add(stringNumbers);
            }
            catch (System.FormatException)
            {
                throw new InvalidDataException("Incorrect matrix");
            }

            this.Size = (rowsCount, columnsCount);
        }

        this.TheMatrix = rows.ToArray();
    }

    /// <summary>
    /// Gets matrix dimensions.
    /// </summary>
    public (int Rows, int Columns) Size { get; }

    /// <summary>
    /// Gets the matrix as an array of arrays.
    /// </summary>
    public int[][] TheMatrix { get; }

    /// <summary>
    /// Creates matrix with positive pseudorandom numbers, each no more than 1000.
    /// </summary>
    /// <param name="rows">first matrix dimension.</param>
    /// <param name="columns">second matrix dimension.</param>
    /// <returns>Matrix object.</returns>
    public static Matrix CreateRandomMatrix(int rows, int columns)
    {
        var randomMatrix = new int[rows][];
        for (var i = 0; i < rows; ++i)
        {
            randomMatrix[i] = new int[columns];
            for (var j = 0; j < columns; ++j)
            {
                randomMatrix[i][j] = random.Next(1000);
            }
        }

        return new Matrix((rows, columns), randomMatrix);
    }

    /// <summary>
    /// Muliplies matrices using multiple threads.
    /// </summary>
    /// <param name="secondMatrixForMultiplication">second argument for multiplication.</param>
    /// <returns>multiplication result, Matrix object.</returns>
    public Matrix ParallelMultiplyMatrices(Matrix secondMatrixForMultiplication)
    {
        this.CheckDimensionsOfMatrices(secondMatrixForMultiplication);
        var numberOfScalarProductMultiplications = this.Size.Columns;

        var resultMatrixRowsNumber = this.Size.Rows;
        var resultMatrixColumnsNumber = secondMatrixForMultiplication.Size.Columns;
        var multiplicationResultMatrix = new int[resultMatrixRowsNumber][];

        var availableThreads = Environment.ProcessorCount;
        var rowsPerThreadCount = (resultMatrixRowsNumber / availableThreads) + 1;

        var threads = new Thread[availableThreads];

        for (var i = 0; i < threads.Length; ++i)
        {
            var locali = i;
            threads[i] = new Thread(() =>
            {
                for (var j = locali * rowsPerThreadCount;
                    j < (locali + 1) * rowsPerThreadCount && j < resultMatrixRowsNumber; ++j)
                {
                    var resultRow = new int[resultMatrixColumnsNumber];
                    for (var k = 0; k < resultMatrixColumnsNumber; ++k)
                    {
                        for (var t = 0; t < numberOfScalarProductMultiplications; ++t)
                        {
                            resultRow[k] += this.TheMatrix[j][t]
                                * secondMatrixForMultiplication.TheMatrix[t][k];
                        }
                    }

                    multiplicationResultMatrix[j] = resultRow;
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        var resultMatrix = new Matrix(
            (resultMatrixRowsNumber, resultMatrixColumnsNumber),
            multiplicationResultMatrix);
        return resultMatrix;
    }

    /// <summary>
    /// Implements standart matrices multiplication.
    /// </summary>
    /// <param name="secondMatrixForMultiplication">second argument for multiplication.</param>
    /// <returns>multiplication result, Matrix object.</returns>
    public Matrix ConsistentMultiplyMatrices(Matrix secondMatrixForMultiplication)
    {
        this.CheckDimensionsOfMatrices(secondMatrixForMultiplication);
        var numberOfScalarProductMultiplications = this.Size.Columns;
        var multiplicationResultMatrix = new int[this.Size.Rows][];
        for (var i = 0; i < this.Size.Rows; ++i)
        {
            multiplicationResultMatrix[i] = new int[secondMatrixForMultiplication.Size.Columns];
        }

        for (var i = 0; i < this.Size.Rows; i++)
        {
            for (var j = 0; j < secondMatrixForMultiplication.Size.Columns; j++)
            {
                for (var k = 0; k < numberOfScalarProductMultiplications; k++)
                {
                    multiplicationResultMatrix[i][j] += this.TheMatrix[i][k]
                        * secondMatrixForMultiplication.TheMatrix[k][j];
                }
            }
        }

        var resultMatrix = new Matrix(
            (this.Size.Rows,
            secondMatrixForMultiplication.Size.Columns),
            multiplicationResultMatrix);
        return resultMatrix;
    }

    /// <summary>
    /// Writes matrix elements into the file.
    /// </summary>
    /// <param name="fileName">file to which the matrix should be written.</param>
    public void SaveToFile(string fileName)
    {
        using StreamWriter fileStream = new(fileName);
        for (var i = 0; i < this.Size.Rows; ++i)
        {
            var lineForFile = new StringBuilder();
            for (var j = 0; j < this.Size.Columns; ++j)
            {
                lineForFile.Append(this.TheMatrix[i][j]);
                if (j != this.Size.Columns - 1)
                {
                    lineForFile.Append(' ');
                }
            }

            fileStream.WriteLine(lineForFile);
        }
    }

    private void CheckDimensionsOfMatrices(Matrix secondMatrixForMultiplication)
        {
            if (this.Size.Columns != secondMatrixForMultiplication.Size.Rows)
            {
                throw new InvalidOperationException("Wrong dimensions of matrices");
            }
        }
    }
