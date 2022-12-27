namespace ParallelMatrixMultiplication;

using System;

/// <summary>
/// Evaluates such values as mathematical expectation, variance of a random variable, standard deviation.
/// </summary>
public static class Evaluation
{
    /// <summary>
    /// Evaluates mathematical expectation of a random variable.
    /// </summary>
    /// <param name="randomVariableValues">Array of random variable values.</param>
    /// <param name="randomVariableValuesProbability">Probability for each random variable value.</param>
    /// <returns>Mathematical expectation.</returns>
    public static double EvaluateMathematicalExpectation(
        double[] randomVariableValues,
        double randomVariableValuesProbability)
    {
        double mathematicalExpectation = 0;
        for (var i = 0; i < randomVariableValues.Length; ++i)
        {
            mathematicalExpectation += randomVariableValues[i];
        }

        mathematicalExpectation *= randomVariableValuesProbability;
        return mathematicalExpectation;
    }

    /// <summary>
    /// Evaluates variance of a random variable.
    /// </summary>
    /// <param name="randomVariableValues">Array of random variable values.</param>
    /// <param name="randomVariableValuesProbability">Probability for each random variable value.</param>
    /// <returns>Variance.</returns>
    public static double EvaluateVariance(
        double[] randomVariableValues,
        double randomVariableValuesProbability)
    {
        var mathematicalExpectation = EvaluateMathematicalExpectation(
            randomVariableValues,
            randomVariableValuesProbability);
        double mathematicalExpectationWithSquaredRandomVariable = 0;
        for (var i = 0; i < randomVariableValues.Length; ++i)
        {
            mathematicalExpectationWithSquaredRandomVariable += Math.Pow(randomVariableValues[i], 2);
        }

        mathematicalExpectationWithSquaredRandomVariable *= randomVariableValuesProbability;

        double mathematicalExpectationSquared = Math.Pow(mathematicalExpectation, 2);
        var variance = mathematicalExpectationWithSquaredRandomVariable - mathematicalExpectationSquared;
        return variance;
    }

    /// <summary>
    /// Evaluates standard deviation of a random variable.
    /// </summary>
    /// <param name="randomVariableValues">Array of random variable values.</param>
    /// <param name="randomVariableValuesProbability">Probability for each random variable value.</param>
    /// <returns>Standard deviation.</returns>
    public static double EvaluateStandardDeviation(
        double[] randomVariableValues,
        double randomVariableValuesProbability)
        => Math.Round(
            Math.Sqrt(EvaluateVariance(
            randomVariableValues,
            randomVariableValuesProbability)),
            2);
}
