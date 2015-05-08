using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// This will be tutorial of extension methods and yield.

# region Extension methods
[TestClass]
public static class Lesson2_Part1
{
    // Extension method is defined in a static method in a static class
    // using "this"-keyword with first parameter:

    public static int AddOne(this int x)
    {
        return x + 1;
    }

    // What this will do is add "AddOne" method for every type of this param (integer).
    // It will exist in the whole namespace.
    // That is a classic April fool to add extension method to type object.
    [TestMethod] public static void L2_P1_UseExtensionMethod()
    {
        int three = 3;
        int four = three.AddOne();

        var nine = 8.AddOne();

        // Why the extension methods are so great?
        // - Leads towards natural execution order.
        // - You can add tools and helper methods to the place that they can be found
        // - They can't touch the internal state of the extended object
        //   -> They don't violate "the open closed principle" like partial classes do.

        // If we use simple classes which have only properties ("POCO"),
        // we could add our business logic as extension method to those classes.

        // When we would return other POCO-classes from those, we could validate
        // that our business process is working like we want to.
            
        // var ok = Invoice.Save().Validate().SendToProcess();
            
        // where Save() would return SavedInvoice, 
        // Validate() would return ValidatedInvoice,
        // and as SendToProcess is extension for only ValidatedInvoice,
        // you could never send un-validated invoice!

    }
        
}
#endregion

# region Yield return
[TestClass]
public class Lesson2_Part2
{
    [TestMethod]
    public void L2_P2_ImperativeVsDeclarative()
    {
        // Imperative code: Your code tells computer what to do and how.
        
        // Declarative code: Your code describes computer what you want. 
        //                   No implementation details.

        // It is better to use declarative code than imperative.

        // Imperative example: 
        // Tell to computer:
        // 10: "Assign an integer to zero."
        // 20: "Print that to console."
        // 30: "Add one to it."
        // 40: "Is that smaller than 5? If yes, then goto 20."
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine(i);
        }

        // Declarative example:
        // Tell to computer: 
        // "Give me numbers from 0-5 in a list and print them":
        // (also I don't care do you use for/foreach or what ever loop, 
        //  and can you do it parallel or not.)
        Enumerable.Range(0, 5).ToList().ForEach(Console.WriteLine);

        // Computers will have more processors in the future and you don't want to know 
        // how to utilize them, just let the computer do the job.

        // "for", "if" and also "yield return" are imperative code. Avoid them and use LINQ instead.
        // But now I assume you don't know LINQ well yet, and understanding yield is important.
        // The reason is that "yield return" will be executed lazily. And LINQ is lazy.

    }

    // "yield return" will add value to collection and continue.
    // So it won't stop like normal return.
    public static IEnumerable<string> YieldValues()
    {
        yield return "Select from list:";

        for (var i = 3; i < 10; i++) // Too bad that C# can't yield array at once
        {
            yield return "Item " + i;
        }
    }

    // But the main thing here is that even this above code returns a list, 
    // I never made a concrete new List<T>.

    // This code is a infinite loop, but as yield return is lazy, it will work just fine:
    public static IEnumerable<int> GetAllPositiveIntegers()
    {
        var i = 0;
        while (true) yield return i++;
    }

    [TestMethod]
    public void L2_P2_YouMayDebugThis()
    {
        // A) From a set of AllPositiveIntegers.
        // B) Filter only set of even numbers.
        // C) From set of even numbers, skip 5 first numbers.
        // D) From this set, take the 5 next even numbers.
        var result = GetAllPositiveIntegers().Where(i => i % 2 == 0).Skip(5).Take(5);

        result.ToList().ForEach(Console.WriteLine);

        // Rather than imperative code, think this as set operations 
        // (elementary school mathematics, category theory)

    }
}
#endregion
