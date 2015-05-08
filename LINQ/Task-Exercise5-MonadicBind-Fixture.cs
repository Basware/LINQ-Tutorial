using System;
using Exercise5;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Exercise task: Do not modify tests. Try to get these passed.
/// Actual code is done to the other project: LINQ-Exercises
/// </summary>

[TestClass]
public class Task_Exercise5_LINQ_CustomContainer_Fixture
{

    [TestMethod]
    public void E5_SelectMany_Test()
    {
        var euros1 = new EUR<decimal>(5.5m);
        Assert.AreEqual(5.5m, euros1.Total);

        var euros2 = new EUR<decimal>(10m);
        Assert.AreEqual(10m, euros2.Total);

        var res1 = from sum1 in euros1
                   from sum2 in euros2
                   select sum1 + sum2 + 10;

        Assert.AreEqual(25.5m, res1.Total);

        var res2 = from sum1 in new EUR<int>(8)
                   from sum2 in new EUR<int>(3)
                   select sum1 * 2 + sum2 * 3 - 2;

        Assert.AreEqual(23, res2.Total);

    }
}


public static class LockExtensions
{
    public static EUR<TOut> SelectMany<TIn, TIn2, TOut>(this EUR<TIn> source,
                                                            Func<TIn, EUR<TIn2>> valueSelector,
                                                            Func<TIn, TIn2, TOut> resultSelector)
    {
        return new EUR<TOut>(MyEurosTask.Bind(source, valueSelector, resultSelector));
    }

}
