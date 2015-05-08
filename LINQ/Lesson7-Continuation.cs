using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// This will be tutorial of Continuation 
// and differences between types IEnumerable<T>, IQueryable<T>, IObservable<T> and how to work with these.
#region Continuation
[TestClass]
public class Lesson7_Part1
{
    [TestMethod]
    public void L7_P1_ContinuationExample()
    {
        // What is really happening in this LINQ?

        // var res1 = from x in xs
        //            from y in ys
        //            let m = x*y
        //            select m + 1;

        // Behind the scene it is translated to something like this:

        // Bind(xs, (x =>
        //   Bind(ys, (y =>
        //     Let(x*y, (m =>
        //       Let(m+1, (res4 =>
        //         Return(res4)))))))))

        // So the last parameter of each command is actually a function containing the next executed step.
        // This has a name: call/cc "call-with-current-continuation"

        // If this wouldn't be used, we would fall back immediately from the monad.
        // And it is not what we want: we want lazy computation.

        // That is the reason e.g. why Take() returns lazy IEnumerable<T> and not the concrete items.
        // If take would execute immediately then this would be a lot of overhead:

        var mySet1000 = Enumerable.Range(0, int.MaxValue).Select(s => { Thread.Sleep(1000); return s; }).Take(500);
        var actual = mySet1000.Skip(1).First();
        Console.WriteLine(actual);

        // ...but as you can test it sleeps just 1 sec.

        // The same holds with databases: You don't want to fetch always everything.
    }

}
#endregion
#region Same LINQ, different monads

[TestClass]
public class Lesson7_Part2
{

    // What is not obvious (and even dangerous) is that the same LINQ can be used in different
    // contexts (and you don't even know where you are):

    // IEnumerable<T> 
    //        - Basic business logics with lists
    //        - Side-effect when exiting the monad: 
    //          Enumerates the lazy evaluations

    // IQueryable<T>:
    //        - Databases
    //        - Side-effect when exiting the monad: 
    //          Creates network traffic between Database and business logic

    // IObservable<T>: 
    //        - Lazy event streams
    //        - Side-effect when exiting the monad: 
    //          Wait for the stream

    // And you should know where you are.

    // For more detailed descriptions (with nice words like isomorphism and duality), 
    // check the links... (from next chapter)

    // Best way with IQueryable<T>s would be:
    // We should construct one SQL to fetch all objects, to make it all. 
    // This way SQL Server may optimize the query.

    // One problem is the context:
    //  - Context holds both: Data schema and the connection.
    //  - As long as we can't separate those two, we have to work with the rules of the connection:
    //    Exit the monad and enumerate before closing the connection.

    // You shouldn't use imperative for/foreach -programming as it will actually create a huge
    // network traffic between business logic and the database.


    // Best way with IObservable<T>s would be:
    // Never exit the monad. 
    // Instead use one .Subscribe()-method which will take a function parameters 
    // (which will be "injected" to the right place).

    [TestMethod]
    public void L7_P2_IeIoIq()
    {
        IEnumerable<int> ie = Enumerable.Empty<int>();

        IQueryable<int> iq = null;
        
        // Note the differences between type conversions:

        //.As... is lazy and won't enumerate
        iq = ie.AsQueryable();
        ie = iq.AsEnumerable();

        //.To... will cause side effect of enumeration (and exit the monad):
        var l = ie.ToList();
        var a = ie.ToArray();

        // Use of IObservable would need Microsoft Reactive Framework (Rx):
        // While the interface is in core .NET, the functionality comes as extension methods 
        // with this library.
        IObservable<int> io = null;

        // Use of IQueryable covered more in EntityFramework... 
    }
    
    // So... LINQ can also be thought as a pipeline over (list) monad. Monad (term from 
    // Haskel programming language) means some kind of container holding some kind of state, 
    // that you as programmer can be unaware of (e.g. is your list in alphabetical order or not?).
    // This state will cause some kind of side effect that is caused when you exit the monad. 
    // The pipeline means that you can combine commands (by function composition): 
    // the output will be ok for another input (as it has the same type).
}

#endregion
