namespace ParallelMatrixMultiplication;

using System;

/// <summary>
/// Evaluates such values as mathematical expectation, variance of a random variable, standard deviation.
/// </summary>
public class Evaluation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Evaluation"/> class.
    /// </summary>
    /// <param name="randomVariableValues">array of random variable values.</param>
    /// <param name="randomVariableValuesProbability">probability for each random variable value.</param>
    public Evaluation(double[] randomVariableValues, double randomVariableValuesProbability)
    {
        if (randomVariableValuesProbability == 0)
        {
            throw new ArgumentException("Incorrect probability for evaluating");
        }

        this.RandomVariableValues = randomVariableValues;
        this.RandomVariableValuesProbability = randomVariableValuesProbability;
    }

    /// <summary>
    /// Gets array of random variable values.
    /// </summary>
    public double[] RandomVariableValues { get; }

    /// <summary>
    /// Gets probability for each random variable value.
    /// </summary>
    public double RandomVariableValuesProbability { get; }

    /// <summary>
    /// Evaluates mathematical expectation of a random variable.
    /// </summary>
    /// <returns>mathematical expectation.</returns>
    public double EvaluateMathematicalExpectation()
    {
        double mathematicalExpectation = 0;
        for (var i = 0; i < this.RandomVariableValues.Length; ++i)
        {
            mathematicalExpectation += this.RandomVariableValues[i];
        }

        mathematicalExpectation *= this.RandomVariableValuesProbability;
        return mathematicalExpectation;
    }

    /// <summary>
    /// Evaluates variance of a random variable.
    /// </summary>
    /// <returns>variance.</returns>
    public double EvaluateVariance()
    {
        var mathematicalExpectation = this.EvaluateMathematicalExpectation();
        double mathematicalExpectationWithSquaredRandomVariable = 0;
        for (var i = 0; i < this.RandomVariableValues.Length; ++i)
        {
            mathematicalExpectationWithSquaredRandomVariable += Math.Pow(this.RandomVariableValues[i], 2);
        }

        mathematicalExpectationWithSquaredRandomVariable *= this.RandomVariableValuesProbability;

        double mathematicalExpectationSquared = Math.Pow(mathematicalExpectation, 2);
        var variance = mathematicalExpectationWithSquaredRandomVariable - mathematicalExpectationSquared;
        return variance;
    }

    /// <summary>
    /// Evaluates standard deviation of a random variable.
    /// </summary>
    /// <returns>standard deviation.</returns>
    public double EvaluateStandardDeviation()
        => Math.Round(Math.Sqrt(this.EvaluateVariance()), 2);
}
