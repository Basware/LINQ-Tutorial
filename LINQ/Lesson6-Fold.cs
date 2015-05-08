using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// This will be tutorial of how to do Aggregates: Fold.

#region Fold: Aggregate

[TestClass]
public class Lesson6_Part1
{
    [TestMethod]
    public void L6_P1_AggregateIsFold()
    {
        // Aggregate is a FOLD / REDUCE. 

        // It will "travel" through the container (monad) 
        // and it will evaluate the state (and cause the side effects) that the monad capsulates.

        // Also this one's signature looks pretty bad:
        // public static TAccumulate Aggregate<TSource, TAccumulate>(
        //      this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)

        // xs -> a -> (a -> x -> a) -> a

        // The idea is that for this you pass:
        // 1) Of course the collection that it extends
        // 2) Some initial seed (that becomes the accumulator)
        // 3) "How to replace": Function how to add each item, one by one, to the accumulator

        // Let's revisit the previous example:
        var result2 = Enumerable.Range(0, 10).Where(x => x > 5 && x < 8);

        // Roll: Sum:
        Console.WriteLine("Sum " + result2.Sum());

        // Sum is shortcut for:
        Console.WriteLine("Sum " + result2.Aggregate(0, (a, s) => a + s));
        // and 0 is redundant:
        Console.WriteLine("Sum " + result2.Aggregate((a, s) => a + s));

        // The algorithm:
        // 1) Initial: a = 0
        // 2) Next item: s = 6 
        // 3) a = (a + s) = 0 + 6 = 6
        // 4) Next item: s = 7
        // 5) a = (a + s) = 6 + 7 = 13
        // So the result is 13.

        // Roll: Count:
        Console.WriteLine("Count " + result2.Count());
        // ...is shortcut for:
        Console.WriteLine("Count " + result2.Aggregate(0, (a, s) => a + 1));

        // But you can do a lot more with the Fold:
        // 1 * 2 * 3 * 4 * 5 * 6 * 7 =
        Console.WriteLine("7! is " + Enumerable.Range(1, 7).Aggregate(1, (a, s) => a*s));

        // Reverse:
        var reverse = Enumerable.Range(1, 5).Aggregate(Enumerable.Empty<int>(),
                                                       (a, s) => Enumerable.Repeat(s, 1).Concat(a));

        reverse.ToList().ForEach(Console.WriteLine);
        Console.WriteLine("---");

        // Filter where even:
        var where = Enumerable.Range(1, 6).Aggregate(Enumerable.Empty<int>(),
                                                     (a, s) => s%2 == 0 ? a.Concat(Enumerable.Repeat(s, 1)) : a);

        where.ToList().ForEach(Console.WriteLine);

        // Core C# is missing some operations:
        //  - Fold left versus fold right: from which end we will start the aggregation
        //  - Unfold / .Generate(): generic way to generate IEnumerable from a function

        // Built-in C# uses Aggregate only for IEnumerable<T>. But it can be used to other data structures also.
        // This is left as a Catamorphism-exercise.
    }
}
#endregion

#region Inside Aggregate: Accumulator and tail recursion
[TestClass]
public static class L6_P2_TailRecursion
{
    // So, why do Aggregate need initial value? Why can't it just do the thing?

    // Inside aggregate is actually a recursive loop.
    
    // How recursion handles the lists:
    // A) Stop-condition, when to stop and return.
    // B) Calculate value for one item.
    // C) Call the recursion with other items.

    // Let's look at this recursive loop:
    public static int CalculateSum(this IEnumerable<int> xs)
    {
        if (!xs.Any()) return 0;
        var sum = xs.First() + CalculateSum(xs.Skip(1));
        return sum;
    }

    // Then test that sum is working...
    [TestMethod]
    public static void L6_P2_SumTest()
    {
        // This will actually work.
        var result2 = Enumerable.Range(1, 100).CalculateSum();
        Console.WriteLine(result2);  // 5050

        // But...evaluating result3 would raise new System.StackOverflowException():
        var result3 = Enumerable.Range(1, int.MaxValue).CalculateSum();
        // (...if you would wait long enough...)

        // The reason is that computer (execution stack) has to remember every
        // returning value to calculate this addition operation to sum-variable.
    }
    // .NET has tail recursion optimization. What this means is that
    // StackOverflowException can be avoided if the last call is
    // the return call (and not the addition operation):

    // So this would not cause StackOverflowException:
    public static int CalculateSumWithAccumulator(this IEnumerable<int> xs, int accumulator)
    {
        if (!xs.Any()) return accumulator;
        var sum = accumulator + xs.First();
        // (Still maybe OverflowException, but now the problem is user, not recursion.)
        return CalculateSumWithAccumulator(xs.Skip(1), sum);
    }
    // But now you need the initial value to this sum when you call the method:
    [TestMethod]
    public static void L6_P2_SumTest2()
    {
        var result4 = Enumerable.Range(1, 100).CalculateSumWithAccumulator(0);
        Console.WriteLine(result4);  // 5050
    }

    // That is the reason for the initial parameter.

    // From this CalculateSumWithAccumulator, if you extract the addition operation "+" 
    // to be a function parameter, you have basically created your own (simplified) .Aggregate():
    public static TR MyAggregate<T1, TR>(this IEnumerable<T1> xs, TR accumulator, Func<TR, T1, TR> func)
    {
        if (!xs.Any()) return accumulator;
        var sum = func(accumulator,xs.First());
        return MyAggregate(xs.Skip(1), sum, func);
    }
}

#endregion
