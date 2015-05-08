using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// This will be tutorial of the real LINQ!

#region Some typical LINQ methods
[TestClass]
public class Lesson5_Part1
{
    // LINQ methods have three kinds of operations:
    // 1. Wrap: Wrappers creates container (IEnumerable) by wrapping type T to the container (IEnumerable)
    //    - eg. Enumerable.Repeat("test", 1) or Enumerable.Empty()
    //    - Also called as Anamorpihsm

    // 2. Shape: Queries apply operations to the container (IEnumerable) 
    //           to make a new container of the same type.
    //    - Evaluations of these are lazy: they will not execute immediately!
    //    - eg. Select, SelectMany
    //    - Continuation: stay in the context!

    // 3. Roll: Aggregates returns something, to get out of container, by folding
    //    - (Finnish: "Koontifunktio")
    //    - e.g. Count, Min, Max, Aggregate, foreach, ToList, ToLookup (=dictionary), ToArray, ...
    //    - Also called as Catamorpihsm

    // Actually, the core operations of LINQ are: SelectMany and Aggregate. 
    // Every other operation is just a shortcut to make using LINQ easier.

    [TestMethod]
    public void L5_P1_FirstWarningOfRoll()
    {
        // Be careful when using this third type of functions:
        // The only place which makes side effect is when you exit the context.
        // For example: You don't want to call .ToList() to previous GetAllPositiveIntegers()

        // .ToList() to database items or user to fetch a lot from database
        // .ToList() to async computation would block the thread to wait.
        var r1 = Enumerable.Range(0, 5)
                    .Select(t => { Thread.Sleep(1000); return t;});

        Console.WriteLine("Before wait");
        var r2 = r1.ToList();
        Console.WriteLine("After wait");
    }

    // Before going deeper with these two operations, let's look the most common operations:
    [TestMethod]
    public void L5_P1_LibraryMethdods()
    {
        // Wrap: First parameter to these extension method is always the collection.
        var result1 = Enumerable.Range(0, 10);

        // Shape: Where is a FILTER for just wanted items:
        var result2 = result1.Where(x => x > 5 && x < 8);
        // Second parameter is a "is-ok? -function" foreach item.

        // From now on: You should NOT use list.RemoveAt(). Instead filter only items you want!

        // Shape: Select is a MAP / PROJECTION, from type to another type:
        var result3 = result2.Select(x => "item " + x);
        // Second parameter is a "change function" foreach item.

        // Shape: .Skip(n) and .Take(n) are usually used as paging functionality:
        var result4 = result3.Skip(1).Take(2);

        // With Zip-method you can merge two lists as you like:
        var result5 = result1.Zip(result1.Reverse(), Tuple.Create);
        result5.ToList().ForEach(Console.WriteLine);

        // All of these are also shape-operators:

        // Some collection operators:
        var c1 = Enumerable.Range(1, 3); //  1, 2, 3
        var c2 = Enumerable.Repeat(3, 3); // 3, 3, 3

        var r1 = c1.Union(c2);     // 1 2 3, object equality is checked through GetHashCode()
        var r2 = c1.Intersect(c2); // 3
        var r3 = c1.Except(c2);    // 1 2

        // Other LINQ methods that you should guess by name:
        var r4 = c1.Distinct(); // removes doubles
        var r5 = c1.TakeWhile(x => x == 2);

        // C# is strongly typed language, so try to avoid: .Cast<T>() and .OfType<T>()
        // .AsParallel() would cause parallel execution for the collection (if possible).
        // But then you should not use side effects (like ref values) in your code...
        // You can Google more on "parallel computing" (and "message passing", etc.).

        // In general it is better to always avoid side effect and don't reuse variables
        // and (also inside lambdas) prefer returning a new object rather than modify existing one.

        Console.WriteLine("---");
        r4.ToList().ForEach(Console.WriteLine);
        Console.WriteLine("---");


        // Now, how to exit the monad:

        // Roll: Count:
        Console.WriteLine("Count " + result2.Count());

        // Roll: On numerical enumerables: Sum, Average, etc:
        Console.WriteLine("Sum " + result2.Sum());

        // Roll: how will we get out something from the container (IEnumerable<T>):
        // ToList() or ToArray() or just manually going through foreach enumeration
        result3.ToList().ForEach(Console.WriteLine);
        
        // or as aggregate:
        // Don't worry, we will discuss more about Aggregates in the next lesson...
        result3.Aggregate((a, s) => { Console.WriteLine(a + " and " + s); return s;});

        // Note that the usual way to check if IEnumerable (result4.Count() > 0) is bad 
        // as it enumerates the whole list just to get a value of greater than zero. Better is: 
        var isTrue = result4.Any();

    }

}
#endregion

#region Monadic Bind: SelectMany
[TestClass]
public class Lesson5_Part2
{
    [TestMethod]
    public void L5_P2_SelectMany()
    {
        // You may have used in your history a code like this:
        // foreach (var x in xs)
        //    foreach (var y in ys)  
        // where ys might take x as a parameter or not.
        
        // There is an operator in LINQ, called SelectMany.
        // Usually it is used through the syntactic sugar form:
        // (Note the similarity to foreach)
        // var res = from x in xs
        //           from y in ys
        //           ....

        var res1 = from i in Enumerable.Repeat(3, 2) // 3, 3
                   from j in Enumerable.Range(1, 2) //  1, 2
                   select i + j; // (3+1), (3+2), (3+1), (3+2)

        res1.ToList().ForEach(Console.WriteLine); // 4, 5, 4, 5
        Console.WriteLine("---");

        // Non-syntactic-sugar -form:
        var res2 = Enumerable.Repeat(3, 2)
                    .SelectMany(i => Enumerable.Range(1, 2).Select(j => i + j));

        // The i variable is bind to something that can be referenced inside the inner Select lambda.

        // The result is not IEnumerable<IEnumerable<T>> but only IEnumerable<T> so this means
        // that the containers has been flattened. Actually if you would just say
        // .SelectMany(i => i) it would just have flatten the structure:

        var coll = new[] {new[] {1, 2, 3}, new[] {4, 5, 6}};
        coll.SelectMany(i => i) // Flattened as: {1,2,3,4,5,6}
            .ToList().ForEach(Console.WriteLine);

        // SelectMany has an ugly method signatures, e.g.:
        // public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, 
        //                                                    Func<TSource, IEnumerable<TCollection>> collectionSelector, 
        //                                                           Func<TSource, TCollection, TResult> resultSelector);

        // xs -> (x -> ys) -> (x -> y -> r) -> r
    }
}
#endregion

#region Missing PatternMatching
[TestClass]
public class Lesson5_Part3
{
    [TestMethod]
    public void L5_P3_CSharpWorkarounds()
    {
        // Let's think of this code:
        var set = Enumerable.Repeat(2, 1)
                        .Select(i => { Console.WriteLine("Side effect"); return i;});

        var r1 = from item in set where item % 2 == 0 select "even";
        var r2 = from item in set where item % 2 != 0 select "odd";

        Console.WriteLine("Don't want to execute side effect before this!");
        r1.ToList().ForEach(Console.WriteLine);
        r2.ToList().ForEach(Console.WriteLine);
        Console.WriteLine("I would like to execute only one side effect!");
        Console.WriteLine("---");
        // How can I not create the double side effect without aggregate method?

        // I would like to say in my select:
        // if (item % 2 == 0) select "even";
        // else if (item % 2 == 1) select "odd";
        // else if ...

        // This is called pattern matching
        // and some advanced languages have it, but not C#.
        // So for now, we just have to live with it...
        // (although authors like Matthew Podwysocki and Bart de Smet have 
        //  imported this to C#, but the implementations are not lazy/type-safe)

        // There are some workarounds:
        // 1) Just accept that enumeration is be done twice or side effect too early.

        // 2) Or if we have a very small condition, we might use temporary variable:
        var r3 = from item in set
                 let temp = item % 2 == 0 ? "even" : "odd"
                 select temp;
        Console.WriteLine("Don't want to execute side effect before this!");
        r3.ToList().ForEach(Console.WriteLine);
        Console.WriteLine("---");

        // 3) Or we could use separate function:
        Func<int, string> selector = i => {
                if (i%2 == 0) {
                    return "even";
                } else {
                    return "odd";
                }
            };
        var r4 = from item in set
                 select selector(item);
        Console.WriteLine("Don't want to execute side effect before this!");
        r4.ToList().ForEach(Console.WriteLine);
        Console.WriteLine("---");
    }
}
#endregion

#region LINQ... extensions and yields?
[TestClass]
public static class Lesson5_Part4
{
    // Actual LINQ methods for IEnumerable<T> could be pretty simple. This is Where:
    public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> items, Func<T, bool> condition)
    {
        foreach (var item in items)
        {
            if (condition(item)) yield return item;
        }
    }

    [TestMethod]
    public static void L5_P4_InsideLinq()
    {
        var xs = Enumerable.Range(1, 10).MyWhere(x => x % 2 == 0);
        xs.ToList().ForEach(Console.WriteLine);
        // As you can see, I can use it instead of LINQ Where.
    }
}
#endregion

#region LINQ to my objects
[TestClass]
public class Lesson5_Part5
{
    // Usually LINQ is used over IEnumerable, but it can be implemented on any type:
    // A) Either implement interface IQueryable<T> in your class
    // B) Or just implement some methods with same signatures, extension or non-extension

    // This is demo of container (monad) hiding the state:
    // The developer programs inside the monad as usual,
    // and is unaware of state (extra bottle)
    // when rolling out of the context:
    class Bottle
    {
        //LINQ will call this method. No matter if it is extension or not:
        public int Select(Func<string, int> howManyBottles)
        {
            // We have coke in the bottle.
            // Also we have this hidden side effect that we give an extra bottle:
            return howManyBottles("coke") + 1;
        }
    }

    [TestMethod]
    public void L5_P5_LinqToMyMonad()
    {
        // User wants 2 bottles of cola, or just one if it is pepsi:
        var res1 = from cola in new Bottle()
                   select cola == "pepsi" ? 1 : 2;
        
        Console.WriteLine(res1); // 3
    }

    // So, actually, now you have three way to do generic "containers" 
    // and they differ mainly how they expose their internal state:

    // Container: |  Type      |   Monad        |  Class
    // State:     |  No state  |   Not exposed  |  Exposed through methods

    // For side-effect free (=bug-free) code, consider avoiding exposing the internal state...
}
#endregion