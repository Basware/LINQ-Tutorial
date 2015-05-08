using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// This will be tutorial of how to make functions using type parameters.
#region Types and classes
[TestClass]
public class Lesson4_Part1
{
    // What is a type?
    // Type is basically a struct or class. 

    // But "type" doesn't have internal state like classes. They are immutable.
    // Classes expose their internal state through methods/functions and that is what makes OO painful.

    // When there are no side effects, we can know the functionality of the program by just type.
    // Some basic examples:
    // What else could this be than get first or last item from list: Func<IEnumerable<T>, T>
    // What else could this be that reverse the list: Func<IEnumerable<T>, IEnumerable<T>>
    // This was simple, but there is paper about this in the links that describes non-simple cases also.
    // Type system is actually semi-automatic-program-correctness-checker!
    // (Types are for the compiler, not for you.)

    [TestMethod]
    public void L4_P1_TypeVsClass()
    {
        // Int is a type:
        int x = 3;

        // This will always return the same:
        var r1 = x + 3;

        // Random is a class that exposes it's internal state through its methods/functions:
        var myRandom = new Random();
        var r2 = myRandom.NextDouble();
        var r3 = myRandom.NextDouble();
        // How can I rely on that .NextDouble() will always return the same?
        // I guess I can't. If I write a test and hope for the best...?
    }
}

#endregion

#region Generics and type parameters
[TestClass]
public class Lesson4_Part2
{

    // Type parameters are often called as "Generics" as the code is generic for several types

    // You have probably already used type parameterized code like:
    private List<int> integers;
    private Func<int> functions;

    private Tuple<int, int> tuple;
    
    // Wait... What is tuple?
    // A list  is "n" times "1 type" of items
    // A tuple is "1" times "n type" of items
    // It is like a simple container object.
    private Tuple<int, string, DateTime, int> tuple2 = Tuple.Create(1, "hi", DateTime.Now, 17);

    // But back to the topic:
    // Now, this is how you make type parameter in a class
    internal class MyClass<T>
    {
        internal T Item { get; set; }
    }

    // T can be any type and this class is a container that just holds this type.
    [TestMethod]
    public void L4_P2_GenericUsage()
    {
        var i1 = new MyClass<int> {Item = 3};
        var i2 = new MyClass<string> {Item = "hello"};

        // Note that unlike object, this type of T will remain the same in the using code.
        // I don't have to cast:
        var v = i1.Item + 4;

        Console.WriteLine(v + " " + i2.Item);
    }
    // The great thing about generics is that you get type errors in the compile time, 
    // not in the runtime (like cast or reflection).

    // Type parameter can also be on the method:
    internal static T MyMethod<T>(T x)
    {
        return x;
    }

    private int r1 = MyMethod<int>(4);
    // If the method has only one type parameter, the compiler can figure it out, and it is redundant:
    private int r2 = MyMethod(5);
    private string r3 = MyMethod("hello");

    // You can define some constraints for T. This is more useful if we know the least common interfaces
    // that those parameters have to be:
    internal static T MyMethod2<T>() where T: class, IComparable, ICloneable, new()
    {
        return new T();
    }

    // where T: new() means it can be constructed. This means you can create "new T()".
    // where T: class means it has to be a class and not a struct. This means you can do "A as T" cast.
    // where T: struct means it has to be a struct.

    // You can have many parameters:
    internal static TResult RandomSwitch<TInput, TResult>(TInput x, TInput y)
            where TInput : TResult
    {
        return new Random().NextDouble() > 0.5 ? x : y;
    }
    
    [TestMethod]
    public void L4_P2_SomeMoreGenerics()
    {
        var l1 = new [] {1, 2, 3};
        var l2 = new [] {4, 5, 6};
        var res = RandomSwitch<IList<int>, IEnumerable<int>>(l1, l2);
        res.ToList().ForEach(Console.WriteLine);
    }

    // One ugly point of C# is that you can't use var everywhere and 
    // you will serve the type system, not the type system you:
    private List<Tuple<Func<IEnumerable<int>, int, Dictionary<int, int>>, Action<int>, Tuple<int,int>>> myThing;

    // ... or...
    private Func<Func<int, List<int>, string>, Func<int, List<int>>, int, string>

        // Also, this is an easy function, but first you notice the ugly type-syntax above:
        S = (x, y, z) => x(z, y(z));

}
#endregion

