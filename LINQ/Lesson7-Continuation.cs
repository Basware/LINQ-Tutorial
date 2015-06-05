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
}
#endregion
#region Entity Framework in brief

// Example of "domain model" POCO object, for OR-mapping, won't have any logic.
// They just model the data shape and can be in separate dll. 
// This example has the attributes disabled as EntityFramework (from NuGet) and 
// System.ComponentModel.DataAnnotations.Schema are not referenced.
//[Table("MyTable")]
public class MyObject
{
    /* [Column("Id"), Required, Key]   */ public string Id { get; set; }
    /* [Column("MyColumn1"), Required] */ public int MyProperty { get; set; }
    /* [Column("MyColumn2"), Required] */ public bool MyCondition { get; set; }
}

[TestClass]
public class Lesson7_Part3_EntityFramework
{
    // LINQ-command-expression-trees can be compiled to C# or what ever language.
    // The core idea of Entity Framework (EF) is to compile those to SQL.

    // To be efficient, you should:
    //   - Avoid network traffic between business logics (BL) and database (DB).
    //     - When you exit the IQueryable-monad, you cause the traffic.
    //   - Model (multiple) small DbContext classes and entity objects. 
    //     - DBContext have 2 roles: "contain the data model" and "manage the connections"
    //     - Usually large means slow. Avoid opening multiple at once.
    //   - Disable entity tracking when possible. Use AddRange and RemoveRange.
    //     - Execute just once, and know when. Don't hammer the database.
    //   - Don't joining/grouping/unioning table to itself. Warning: Include() causes joins.
    //   - Use Key- and Required-attributes and avoid nulls in DB-table-desing. 

    // These commands won't cause evaluation, so you stay in your expression tree:
    //     Select, SelectMany, Where, GroupBy, Skip, Take
    // These commands will exit the queryable monad and as a side effect, 
    // cause an evaluation ("reify") that will execute your LINQ as SQL-command:
    //     Aggregate, ToList, ToListAsync, Any, AnyAsync, All, Count, First, FirstAsync,
    //     Single, SingleAsync, FirstOrDefault, FirstOrDefaultAsync, SingleOrDefault

    // All the commands cannot be listed here. But one rule of thumb is that if you have 
    // a result of IQueryable<T> then you still get your expression tree without evaluation. 
    // There are some exceptions, so you should always profile your SQL-executions.
    // (AsQueryable() won't make your list as SQL but you may use it in your unit tests.)

    // Async-versions of these "reify"-commands will release BL-server thread while DB
    // operates. So you get more performance by concurrency. Your execution thread may change.
    // WCF (and MSTest) supports async/await. Async can't be used as way to do parallelism 
    // inside one EF-context. Note that .NET 4.5.1 has fix for asynchronous transactions.

    // EF doesn't support complex object (or Tuple) inside SQL (but your final return may
    // create one). And C# doesn't support anonymous types as method parameters so your methods 
    // can be long. But still they should have very low count of code-paths.

    // Which one is better?
    //    - from x in xs  from y in ys  where zs.Contains(x)  ...
    //    - from x in xs  where zs.Contains(x)  from y in ys  ...
    // Unfortunately there is no clear answer as the pipeline is:
    // -> EF -> Connection provider -> SQL-server.
    // Each one of these may optimize the SQL even to the format that is too complex
    // for the next optimizer. So again, you have to profile your SQL to find the best for you.

    // For the examples... Imagine that this comes from the database:
    public static IQueryable<MyObject> myObjects = Enumerable.Empty<MyObject>().AsQueryable();

    // The real context is something like this:
    // public class MyDbContext : DbContext {
    //    public MyDbContext() : base("myConnectionString") { }
    //    public DbSet<MyObject> MyObjects; 
    // }

    // Next, some EF-tips: IN-clause and CASE-clause in SQL.
    [TestMethod]
    public void L7_P3_SQL_In()
    {
        // IEnumerable of something, simple objects, like strings, ints, ...
        IEnumerable<int> xs = new[] { 1, 2, 3, 4, 5 }; // Consider splitting if over 500!

        // This is good, makes an SQL IN-clause:
        var resGood =
            from i in myObjects
            where xs.Contains(i.MyProperty)
            select i;

        // This is very bad, will hit x times the database:
        var resBad =
            from x in xs //IEnumerable<T> here is bad as it hits wrong override of SelectMany.
            from i in myObjects
            where x == i.MyProperty
            select "never do this";
    }

    [TestMethod]
    public void L7_P3_SQL_Case()
    {
        // Pattern-match inside flow with conditional operator "?" converts
        // to one SQL-clause, one execution, which is good!
        // You may use temp-variables to keep source code readable.
        // You want this when your condition comes from the database:
        var res =
            from i in myObjects
            let sum = i.MyCondition ? i.MyProperty : (i.MyProperty * 2)
            select new { i.Id, sum };

        // But if your condition is known already (outside your DB-query LINQ)
        var myConditionOutsideSQL = true;
        // then you don't want always-true SQL and may rather use 
        // conditional operator "?" with two separate LINQ clauses like this:
        var res2 =
            myConditionOutsideSQL ?
                from i in myObjects
                select new { i.Id, Property = i.MyProperty }
            :
                from i in myObjects
                select new { i.Id, Property = i.MyProperty * 2 };
    }

    // From a lambda-point of view: x => { y + x } 
    // here x is called "bound variable" and y is a "free variable" or a "captured closure".
    // If you use free variables (defined outside the scope of the lambda) in your LINQ, 
    // always check that they are immutable and will their use cause some extra execution
    // (e.g. if y can be a function-call, y(), then this may cause multiple execution and 
    //  y shouldn't have side-effects.)

}

// Unit-testing EF is possible. But EF won't support full LINQ so you may 
// also want to have integration tests using the DB with some test-data.

public static class Lesson7_Part3_Tests
{
    // Try to code from top-down, give the high level business-oriented picture to
    // the developer when (s)he opens your Visual Studio solution. 
    // If you want to bring your LINQ from outside of the context then use function parameters.

    // But for simplicity, let's say your "high level" production code looks something like:

    // using (var context = new MyDbContext()){
    //    var res = context.MyObjects.AsQueryable()
    //        .Business_Op_1_FetchData()
    //        .Business_Op_2_CalculateMore()
    //        .Business_Op_3_etc();
    //    res.ToListAsync();
    // }

    // Then the logic methods can be separated and unit-tested:    
    internal static IQueryable<MyObject> Business_Op_1_FetchData(this IQueryable<MyObject> myDbItems)
    {
        var result =
            from i in myDbItems
            where i.MyProperty > 10 // some independent business logics
            select i;
        return result;
    }

    // Now this is in the unit test assembly. Actually it won't need any IoC- or mocking-framework.
    [TestMethod]
    public static void L7_P3_UnitTest()
    {
        var data = Enumerable.Repeat(new MyObject { MyProperty = 11 }, 5).AsQueryable();
        var res = Business_Op_1_FetchData(data).ToList();
        Assert.AreEqual(5, res.Count());
    }
}

#endregion

// So... LINQ can also be thought as a pipeline over (list) monad. Monad (term from 
// Haskel programming language) means some kind of container holding some kind of state, 
// that you as programmer can be unaware of (e.g. is your list in alphabetical order or not?).
// This state will cause some kind of side effect that is caused when you exit the monad. 
// The pipeline means that you can combine commands (by function composition): 
// the output will be ok for another input (as it has the same type).
