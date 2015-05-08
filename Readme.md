## LINQ-Tutorial ##

This is LINQ training material for our Senior Software Engineers. 

This tutorial is in form of Visual Studio solution. The best way to use this tutorial is to clone this repository and read the materials from your Visual Studio.

The texts and code-samples are in the form of unit test projects so you can run and test those as you read. 

The practice tasks can be solved by a line or two of code. But some of those are not easy as they test the theory that is written to this tutorial. Practice tasks are unit tests and you should be able to write the code to pass those test without modifying the tests.

---

The structure of this tutorial:

1. [Introduction](LINQ/Lesson1-WhatIsLinq.cs)
  - Details of C# lists
2. [What are the extension methods](LINQ/Lesson2-ExtensionsAndYield.cs)
  - Extension methods
  - Yield return
3. [What are the function parameters](LINQ/Lesson3-Functions.cs)
  - Delegates, Func<T> and lambdas
  - Functions as parameters
  - Expression trees
  - Closure: Capturing variables
4. [Types and type parameters: Generics](LINQ/Lesson4-Types-and-Generics.cs)
  - Types and classes
  - Generics and type parameters
5. [Monads: Map, Select and Select many](LINQ/Lesson5-Monads.cs)
  - Some typical LINQ methods
  - Monadic Bind: SelectMany
  - Missing PatternMatching
  - LINQ... extensions and yields?
  - LINQ to my objects
6. [Catamorphism: Aggregate is Fold](LINQ/Lesson6-Fold.cs)
  - Fold: Aggregate
  - Inside Aggregate: Accumulator and tail recursion
7. [Continuation and types of IE<T> vs. IQ<T> vs IO<T> vs ...](LINQ/Lesson7-Continuation.cs)
  - Continuation
  - Same LINQ, different monads
8. [Links](LINQ/Lesson8-Links.cs)
