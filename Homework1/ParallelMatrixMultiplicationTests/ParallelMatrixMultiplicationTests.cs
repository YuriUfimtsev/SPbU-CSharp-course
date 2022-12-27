namespace ParallelMatrixMultiplicationTests;

using ParallelMatrixMultiplication;

public class ParallelMatrixMultiplicationTests
{
    [Test]
    public void StandartSquareMatricesMultiplicationTest()
    {
        var firstMatrix = new Matrix("StandartSquareMatricesMultiplicationTestData1.txt");
        var secondMatrix = new Matrix("StandartSquareMatricesMultiplicationTestData2.txt");
        var expectedResultMatix = new Matrix("StandartSquareMatricesMultiplicationTestData3.txt");
        var resultMatrixByCoherentMultiplication = firstMatrix.ConsistentMultiplyMatrices(secondMatrix);
        var resultMatrixByParallelMultiplication = firstMatrix.ParallelMultiplyMatrices(secondMatrix);
        Assert.That(resultMatrixByCoherentMultiplication.TheMatrix, Is.EqualTo(expectedResultMatix.TheMatrix));
        Assert.That(resultMatrixByParallelMultiplication.TheMatrix, Is.EqualTo(expectedResultMatix.TheMatrix));
    }

    [Test]
    public void NullMatricesMultiplicationTest()
    {
        var firstMatrix = new Matrix("NullMatricesMultiplicationTestData1.txt");
        var secondMatrix = new Matrix("NullMatricesMultiplicationTestData1.txt");
        var expectedResultMatix = new Matrix("NullMatricesMultiplicationTestData2.txt");
        var resultMatrixByCoherentMultiplication = firstMatrix.ConsistentMultiplyMatrices(secondMatrix);
        var resultMatrixByParallelMultiplication = firstMatrix.ParallelMultiplyMatrices(secondMatrix);
        Assert.That(resultMatrixByCoherentMultiplication.TheMatrix, Is.EqualTo(expectedResultMatix.TheMatrix));
        Assert.That(resultMatrixByParallelMultiplication.TheMatrix, Is.EqualTo(expectedResultMatix.TheMatrix));
    }

    [Test]
    public void MultiplicationOfVectorAndMatrixTest()
    {
        var firstMatrix = new Matrix("MultiplicationOfVectorAndMatrixTestData1.txt");
        var secondMatrix = new Matrix("StandartSquareMatricesMultiplicationTestData1.txt");
        var expectedResultMatix = new Matrix("MultiplicationOfVectorAndMatrixTestData3.txt");
        var resultMatrixByCoherentMultiplication = firstMatrix.ConsistentMultiplyMatrices(secondMatrix);
        var resultMatrixByParallelMultiplication = firstMatrix.ParallelMultiplyMatrices(secondMatrix);
        Assert.That(resultMatrixByCoherentMultiplication.TheMatrix, Is.EqualTo(expectedResultMatix.TheMatrix));
        Assert.That(resultMatrixByParallelMultiplication.TheMatrix, Is.EqualTo(expectedResultMatix.TheMatrix));
    }

    [Test]
    public void MultiplicationOfMatrixAndVectorTest()
    {
        var firstMatrix = new Matrix("StandartSquareMatricesMultiplicationTestData2.txt");
        var secondMatrix = new Matrix("MultiplicationOfVectorAndMatrixTestData2.txt");
        var expectedResultMatix = new Matrix("MultiplicationOfVectorAndMatrixTestData4.txt");
        var resultMatrixByCoherentMultiplication = firstMatrix.ConsistentMultiplyMatrices(secondMatrix);
        var resultMatrixByParallelMultiplication = firstMatrix.ParallelMultiplyMatrices(secondMatrix);
        Assert.That(resultMatrixByCoherentMultiplication.TheMatrix, Is.EqualTo(expectedResultMatix.TheMatrix));
        Assert.That(resultMatrixByParallelMultiplication.TheMatrix, Is.EqualTo(expectedResultMatix.TheMatrix));
    }

    [Test]
    public void IncorrectMultipliedMatricesDimensionsTest()
    {
        var firstMatrix = new Matrix("MultiplicationOfVectorAndMatrixTestData2.txt");
        var secondMatrix = new Matrix("StandartSquareMatricesMultiplicationTestData2.txt");
        Assert.Throws<InvalidOperationException>(() => firstMatrix.ConsistentMultiplyMatrices(secondMatrix));
        Assert.Throws<InvalidOperationException>(() => firstMatrix.ParallelMultiplyMatrices(secondMatrix));
    }

    [Test]
    public void InitializeMatrixWithoutMultipleElementsFromFile()
    {
        Assert.Throws<InvalidDataException>(() => new Matrix("InitializeMatrixWithoutMultipleElementsFromFileData.txt"));
    }

    [Test]
    public void InitializeMatrixWithIncorrectSizeAndDimensions()
    {
        var matrix = new Matrix("MultiplicationOfVectorAndMatrixTestData2.txt");
        Assert.Throws<InvalidDataException>(() => new Matrix((2, 2), matrix.TheMatrix));
    }
}