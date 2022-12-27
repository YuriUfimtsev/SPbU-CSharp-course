using ParallelMatrixMultiplication;

namespace ParallelMatrixMultiplicationTests;

public class EvaluationTests
{
    [Test]
    public void NullProbabilityTest()
    {
        double[] randomVariableValuesArray = { 1, 3, 6 };
        Assert.Throws<ArgumentException>(() => new Evaluation(randomVariableValuesArray, 0));
    }

    [Test]
    public void StandartEvaluationTest()
    {
        double[] randomVariableValuesArray = { 1, 4, 9, 15 };
        var evaluation = new Evaluation(randomVariableValuesArray, 0.25);
        var mathematicalExpectation = evaluation.EvaluateMathematicalExpectation();
        Assert.That(mathematicalExpectation, Is.EqualTo(7.25));
        var variance = evaluation.EvaluateVariance();
        Assert.That(Math.Round(variance, 2), Is.EqualTo(28.19));
        var standardDeviation = evaluation.EvaluateStandardDeviation();
        Assert.That(standardDeviation, Is.EqualTo(5.31));
    }
}

