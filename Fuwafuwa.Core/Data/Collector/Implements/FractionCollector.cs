using Fuwafuwa.Core.Data.Collector.Interface;

namespace Fuwafuwa.Core.Data.Collector.Implements;

public class FractionCollector : ISubjectTaskCollector {
    private readonly HashSet<SubjectData.Level1.SubjectData> _dataSet;
    private readonly ExecuteDataSet.ExecuteDataSet _taskSet;
    private Fraction _fraction;

    public FractionCollector() {
        _fraction = new Fraction(0, 1);
        _taskSet = new ExecuteDataSet.ExecuteDataSet();
        _dataSet = [];
    }

    public void Collect(SubjectData.Level1.SubjectData subjectData) {
        var denominator = 1;
        var currentData = subjectData;
        while (currentData != null) {
            denominator *= currentData.SiblingCount!.Value;

            if (!_dataSet.Contains(currentData)) {
                _taskSet.Combine(currentData.ApplyTasks);
                _dataSet.Add(currentData);
            }

            currentData = currentData.Parent;
        }

        _fraction += new Fraction(1, denominator);
    }

    public bool CheckFinished() {
        return _fraction.IsOne();
    }

    public ExecuteDataSet.ExecuteDataSet? GetTaskSet() {
        if (!_fraction.IsOne()) {
            return null;
        }

        return _taskSet;
    }
}

public class Fraction {
    public Fraction(int numerator, int denominator) {
        if (denominator == 0) {
            throw new ArgumentException("0 is an invalid denominator");
        }

        Numerator = numerator;
        Denominator = denominator;
        Simplify();
    }

    private int Numerator { get; set; }
    private int Denominator { get; set; }

    public bool IsOne() {
        return Denominator == Numerator;
    }

    private void Simplify() {
        var gcd = GCD(Numerator, Denominator);
        Numerator /= gcd;
        Denominator /= gcd;
    }

    private int GCD(int a, int b) {
        while (b != 0) {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    public static Fraction operator +(Fraction a, Fraction b) {
        var numerator = a.Numerator * b.Denominator + b.Numerator * a.Denominator;
        var denominator = a.Denominator * b.Denominator;
        return new Fraction(numerator, denominator);
    }
}