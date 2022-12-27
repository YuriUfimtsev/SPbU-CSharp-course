using ParallelMatrixMultiplication;

namespace ParallelMatrixMultiplicationTests;

public class EvaluationTests
{
    [Test]
    public void StandartEvaluationTest()
    {
        double[] randomVariableValuesArray = { 1, 4, 9, 15 };
        var mathematicalExpectation = Evaluation.EvaluateMathematicalExpectation(randomVariableValuesArray, 0.25);
        Assert.That(mathematicalExpectation, Is.EqualTo(7.25));
        var variance = Evaluation.EvaluateVariance(randomVariableValuesArray, 0.25);
        Assert.That(Math.Round(variance, 2), Is.EqualTo(28.19));
        var standardDeviation = Evaluation.EvaluateStandardDeviation(randomVariableValuesArray, 0.25);
        Assert.That(standardDeviation, Is.EqualTo(5.31));
    }
}

