// LINQ ("Language integrated query")
//
// LINQ is actually "context unaware programming". 
//
// It is usually applied to list manipulation, 
// (as nowadays managing lists or collections is very important when making software)
// but that is not the only point of LINQ.
//
// LINQ deals with immutable data structures: This means that original lists (etc.)
// are not modified. So, there will be no thread-related issues.
//
// C# LINQ is implemented as extension methods taking function parameters.
// C# list base class is IEnumerable<T>
//
// The structure of this tutorial:
//    1) This intro
//       - Details of C# lists
//    2) What are the extension methods 
//       - Extension methods
//       - Yield return
//    3) What are the function parameters
//       - Delegates, Func<T> and lambdas
//       - Functions as parameters
//       - Expression trees
//       - Closure: Capturing variables
//    4) Types and type parameters: Generics
//       - Types and classes
//       - Generics and type parameters
//    5) Monads: Map, Select and Select many
//       - Some typical LINQ methods
//       - Monadic Bind: SelectMany
//       - Missing PatternMatching
//       - LINQ... extensions and yields?
//       - LINQ to my objects
//    6) Catamorphism: Aggregate is Fold
//       - Fold: Aggregate
//       - Inside Aggregate: Accumulator and tail recursion
//    7) Continuation and types of IE<T> vs. IQ<T> vs IO<T> vs ...
//       - Continuation
//       - Same LINQ, different monads
//    8) Links
//
// But before that, first some implementation details of C#:

// Keyboard shortcut to open all the regions at once is Ctrl + M + L

#region Details of C# lists

// I use unit tests so you can run this code as tests.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public static class Lesson1_Part1
{
    [TestMethod]
    public static void L1_P1_CSharpDetails()
    {
        //Let's have a some kind of list of integers:
        int[] xs6; //array  
        List<int> xs5; //a concrete list of integers  
        IList<int> xs4; //a list of integers  
        Collection<int> xs3; // Collection of integers
        ICollection<int> xs2; // Collection of integers
        IEnumerable<int> xs1; //integers that can be enumerated in some way 

        // (And WPF has this ObservableCollection<T> which is based on List and
        // implements INotifyPropertyChanged. Not topic of this tutorial.)

        // The current best practice is to use "var" (or IEnumerable<T>).
        // Code reviews doesn't affect:
        // - You shouldn't do code review by printing the source code.
        // - Compiler already checks the types, they are OK. Please review the functionality.

        // This would be an empty list:
        xs1 = Enumerable.Empty<int>();
        // From now on: You should not use null as an empty list!
        // Even better: avoid use of null totally!

        //Enumerable class is very useful in general...

        // In C# LINQ has two syntaxes, the extension method syntax:
        var result1 = xs1.Where(x => x < 5);

        // and the "SQL-like" syntax:
        var result2 = from x in xs1 where x < 5 select x;

        // These are the same and the later is just syntactic sugar of the first one.

        // This syntax of "from x in xs1" can usually be thought as a loop like "foreach(x in xs1)"...
        // (Or as mathematical/scientific form: ∀x.xs1 -> ...)
    }
}

#endregion
