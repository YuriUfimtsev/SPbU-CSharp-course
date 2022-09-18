using System.Diagnostics;
using System.Text;
using ParallelMatrixMultiplication;

const int N = 10;

void SaveResultsInTable(
    string fileName,
    (double MathematicalExpectation,
    double StandardDeviation)[] coherentMatrixMultiplicationEvaluation,
    (double MathematicalExpectation,
    double StandardDeviation)[] parallelMatrixMultiplicationEvaluation)
{
    using (StreamWriter fileStream = new(fileName))
    {
        var lineForFile = "Matrices sizes: 500*1300x1300*500  1000*1300x1300*1000 1500*1300x1300*1500";
        fileStream.WriteLine(lineForFile);
        var mutableLineForFile = WriteMathematicalExpectationAndStandardDeviationInFile(coherentMatrixMultiplicationEvaluation, true);
        fileStream.WriteLine(mutableLineForFile);
        mutableLineForFile = WriteMathematicalExpectationAndStandardDeviationInFile(parallelMatrixMultiplicationEvaluation, false);
        fileStream.WriteLine(mutableLineForFile);
    }
}

StringBuilder WriteMathematicalExpectationAndStandardDeviationInFile(
    (double MathematicalExpectation,
    double StandardDeviation)[] matrixMultiplicationEvaluation,
    bool isCoherentMultiply)
{
    var mutableLineForFile = new StringBuilder();
    mutableLineForFile.AppendLine(isCoherentMultiply ? "Coherent multiply: " : "Parallel multiply: ");
    for (var i = 0; i < 3; ++i)
    {
        mutableLineForFile = mutableLineForFile.AppendLine($"u={matrixMultiplicationEvaluation[i].MathematicalExpectation}"
            + $";σ={matrixMultiplicationEvaluation[i].StandardDeviation} ");
    }

    return mutableLineForFile;
}

(double MathematicalExpectation, double StandardDeviation)[] FillMatrixMultiplicationEvaluationArray(bool isCoherentMultiplication)
{
    var matrixMultiplicationEvaluation
        = new (double MathematicalExpectation, double StandardDeviation)[3];
    for (var i = 0; i < 3; ++i)
    {
        var stopwatch = new Stopwatch();
        var randomVariableValues = new double[N];
        for (var j = 0; j < N; ++j)
        {
            var firstMultiplier = new Matrix($"..\\..\\..\\Data{i + 1}_firstMultiplier.txt");
            var secondMultiplier = new Matrix($"..\\..\\..\\Data{i + 1}_secondMultiplier.txt");
            stopwatch.Start();
            var multiplicationResult = isCoherentMultiplication
                ? firstMultiplier.CoherentMultiplyMatrices(secondMultiplier)
                : firstMultiplier.ParallelMultiplyMatrices(secondMultiplier);
            stopwatch.Stop();
            double time = Convert.ToDouble(stopwatch.Elapsed.Milliseconds);
            randomVariableValues[j] = time;
        }

        var evaluation = new Evaluation(randomVariableValues, Convert.ToDouble(1 / N));
        matrixMultiplicationEvaluation[i]
            = (evaluation.EvaluateMathematicalExpectation(), evaluation.EvaluateStandardDeviation());
    }

    return matrixMultiplicationEvaluation;
}

var matricesArray = Array.CreateInstance(typeof(Matrix), 3);
var matrixMultipliersArray = Array.CreateInstance(typeof(Matrix), 3);
for (var i = 0; i < 3; ++i)
{
    var firstMultiplier = Matrix.CreateRandomMatrix((i + 1) * 500, 1300);
    var secondMultiplier = Matrix.CreateRandomMatrix(1300, (i + 1) * 500);
    matricesArray.SetValue(firstMultiplier, i);
    matrixMultipliersArray.SetValue(secondMultiplier, i);
    firstMultiplier.SaveToFile($"..\\..\\..\\Data{i + 1}_firstMultiplier.txt");
    secondMultiplier.SaveToFile($"..\\..\\..\\Data{i + 1}_secondMultiplier.txt");
}

var coherentMatrixMultiplicationEvaluation = FillMatrixMultiplicationEvaluationArray(true);
var parallelMatrixMultiplicationEvaluation = FillMatrixMultiplicationEvaluationArray(false);
SaveResultsInTable("..\\..\\..\\Report.txt", coherentMatrixMultiplicationEvaluation, parallelMatrixMultiplicationEvaluation);