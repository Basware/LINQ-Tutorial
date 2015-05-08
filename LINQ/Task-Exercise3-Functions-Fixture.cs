using System;
using Exercise3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Exercise task: Do not modify tests. Try to get these passed.
/// Actual code is done to the other project: LINQ-Exercises
/// </summary>
[TestClass]
public class Task_Exercise3_Functions_Fixture
{
    delegate T OwnR<T>(OwnR<T> me);
    [TestMethod]
    public void E3_Functions_FactorialCalculation_Test()
    {
        OwnR<Func<Func<Func<int, int>, Func<int, int>>, Func<int, int>>> Y = y => f => x => f(y(y)(f))(x);
        var fixedPoint = Y(Y);
        var result = fixedPoint(MyFunction.CalculateFactorial());
        
        Assert.AreEqual(1, result(1));
        Assert.AreEqual(2, result(2));
        Assert.AreEqual(6, result(3));
        Assert.AreEqual(24, result(4));
        Assert.AreEqual(120, result(5));
        Assert.AreEqual(720, result(6));
        Assert.AreEqual(5040, result(7));

        // This is Y. See links for more info about S-, K-, I-, and Y-combinator

        // Y combinator is defined as (Wikipedia): 
        // g(f) = f(g(f))
        // f is the recursion and g is your function to do the business logic (=stop condition and call f)...
    }
}
