using System;
using System.IO;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// This will be tutorial of functions.

# region Delegates, Func<T> and lambdas
[TestClass]
public class Lesson3_Part1
{

    // Now, this is a "method", or a "function":
    public static int AddIntegers(int x, int y)
    {
        return x + y;
    }

    // The reason I made it static is just that I don't have to 
    // create new Part1_Basics() every time.
    // I assume you already know how to avoid unwanted singleton pattern:
    // Static properties are bad as they are common to all users.

    // As you know, you call this function from another like this:
    [TestMethod]
    public void L3_P1_UseFunction()
    {
        var result = AddIntegers(4, 5);
        Console.WriteLine(result); // 9
    }

    // You can assign function as variable and use it like a variable:
    // (Like old C-language function pointer.)

    private Func<int, int, int> AddIntegersFunc1 = AddIntegers;

    // What is Func? The syntax is: first input parameters, then output.
    // This will take two times int in and give one int out. 
    // In other languages the type of this func would be expressed as: int -> int -> int

    // (You could also think it as a class with just one important 
    //  method: .Invoke() which is called with "()")

    // Now you can use it more generally:
    [TestMethod]
    public void L3_P1_UseFunction2()
    {
        var sum = AddIntegersFunc1;
        Console.WriteLine(sum(2, 4)); //6
        //Apply many times
        Console.WriteLine(sum(sum(1, 2), 4)); //7

        // As you know, the execution order is some how un-natural here:
        // "first execute the later" (inner) sum.
    }

    //This: public static int AddIntegers(int x, int y) { return x + y; }
    //can also be written as:
    private Func<int, int, int> AddIntegersFunc2 = delegate(int x, int y) { return x + y; };

    // But now, we already know the type of the function (int -> int -> int)
    // so we don't need to specify it in the delegate again. 
    // Also "delegate" could be replaced with arrow "=>" which is the same:
    private Func<int, int, int> AddIntegersFunc3 = (x, y) => { return (x + y); };

    // If you have just one return statement inside { } , you can replace it with the statement:
    private Func<int, int, int> AddIntegersFunc4 = (x, y) => x + y;

    // This is called a lambda expression.
    // (Or as mathematical/scientific form: AddIntegersFunc4 = λx.λy.x+y)

    // When debugging, the registration comes first, the real execution comes later.
    // You can add a breakpoint inside a lambda:
    // Select the body with mouse and 
    // - From mouse right menu, Breakpoint -> Insert Breakpoint
    // - Or just press F9

    // One stupid limitation of C# is that void is not a type.
    // So Func<T> returning a void has to be a different class. It is Action<T>.
    // When Action<T> or Func<T> has only one parameter, 
    // like Action<int> WriteInteger = v => { Console.WriteLine(v); }, it can be shortened:
        private Action<int> WriteInteger = Console.WriteLine;


    // So they are used as usual:
    [TestMethod]
        public void L3_P1_UseFunction3()
    {
        var sum = AddIntegersFunc4;
        WriteInteger(sum(2, 4)); //6
        //Apply many times
        WriteInteger(sum(sum(1, 2), 4)); //7
    }

}
#endregion

#region Functions as parameters
[TestClass]
public class Lesson3_Part2
{
    // So, now you know this:
    Func<int, int, int> AddIntegers = (x, y) => x + y;

    // Let's do another:
    Func<int, int, int> MultiplyIntegers = (x, y) => x * y;

    // We can of course use functions as input and output parameters:
    void DoWith7and8(Func<int, int, int> addFunction)
    {
        int result = addFunction(7,8);
        Console.WriteLine(result);
    }

    [TestMethod]
    public void L3_P2_ExtractLogics()
    {
            DoWith7and8(AddIntegers);
            DoWith7and8(MultiplyIntegers);
    }

    // If you have a function as static property, you can do a simple inversion of control (IoC):
    // MyClass.Functionality = AddIntegers;
    // var use = new MyClass().CallSomethingThatUsesFunctionality();

    // Now, let's take this a bit further and use function as output parameter:
    Func<int, int> PartialApplication(int x)
    {
        Func<int, int> partialApplication = y => x + y;
        return partialApplication;
    }

    [TestMethod]
    public void L3_P2_UsePartial()
    {
        // Now, this returns me a function (int -> int):
        var addThree = PartialApplication(3);

        // Which I can apply like this:
        var eight = addThree(5);
        var ten = addThree(7);
        var eightteen = eight + ten;
        Console.WriteLine(eightteen);

        // I could also directly call this:
        PartialApplication(5)(8);

        // And we can also write the same partial application with a simple lambda:
        Func<int, Func<int, int>> PartialApplication2 = x => y => x + y;
        var r = PartialApplication2(25)(17);
        Console.WriteLine(r);
    }

    // With partial application you can assign the function without all the parameters.
    // It won't be yet executed (so this is different than optional parameters).

    // Note: It is usually better to use method parameters and not global or class variables.
    // e.g. Func with global variables captures them and will give Garbage Collector some hard time!

    // So... The difference of these functions by type:
    // f1:  a -> b -> c    :  Func<a, b, c>        : f1 is just a normal function
    // f2:  a -> (b -> c)  :  Func<a, Func<b, c>>  : f2 partial application (to avoid boilerplate code)
    // f3: (a -> b) -> c   :  Func<Func<a, b>, c>  : f3 when don't know how to create new B

}
#endregion

#region Expression trees
[TestClass]
public class Lesson3_Part3
{
    [TestMethod]
    public void L3_P3_Expression()
    {
        // The same function body can be translated to two different type:
        Func<int, int>
                                        myFunc = i => i + 5;
        Expression<Func<int, int>>
                                  myExpression = i => i + 5;

        //  Basically you can think of expression being a function that has not been compiled yet.
        // You could compile expression into function:
        myFunc = myExpression.Compile();

        // But that is not usually what you directly want.
  
        // Why to have expressions?
        //  - A compiler is a computer program that transforms 
        //    source code written in a programming language 
        //    into another computer language (source: Wikipedia)
        //  - Usually your code is compiled to CIL (Common Intermediate Language, you can Bing/Google more). 
        //    But in some cases you want to compile your code to something else than executable program.
        //    (Or write your own compiler.)

        // You could want to e.g. compile expression into SQL.

        // One nice thing is that expressions have nice .ToString():
        Console.WriteLine(myExpression); // i => (i + 5)


        // Let's modify the expression before compile just to show that this is possible :
        var body = (BinaryExpression)myExpression.Body;
        var modifiedExpression =
            Expression.Lambda<Func<int, int>>(
                Expression.MakeBinary(ExpressionType.Subtract, body.Left, body.Right), myExpression.Parameters);

        Console.WriteLine(modifiedExpression);  // i => (i - 5)

        var test = modifiedExpression.Compile();
        Console.WriteLine(test(6)); // 1

        // This should be enough to give you the basic idea:
        // Usually Func<...> is enough, but there are places where Expression<Func<...>>s are needed.
        
        // This is a form of homoiconicity: programs and program structure are the same thing.
        // Compared to reflection: expressions are more type-safe than reflection... Reflection always fails at runtime.
    }
}
#endregion

#region Closure: Capturing variables

[TestClass]
public class Lesson3_Part4
{
    Func<string, string> MyMethod()
    {
        // If we have a high resource consumption resource, like IDisposable
        System.IO.Stream s = new MemoryStream();
        // then we should know how Garbage Collector works.
        
        // This "s" is of course defined in this MyMethod() and usually
        // Garbage Collector can dispose it when we exit the method.

        // But if "s" is captured to a function:
        Func<string, string> myfunc = y => y + s.ToString();
        // the GC can't dispose it before all the references to 
        // both MyMethod() and myfunc() are gone.

        // This may be not so obvious if the method is async or if I return it away from here:
        if (new Random().NextDouble() > 0.6) return myfunc;

        // So the point is this: when you do any lambda (not just Func<T>), 
        // you should know your captured parameters.
        // Avoid capturing disposables and large objects like "this".
        return h => h;
    }
    [TestMethod]
    public void L3_P4_ClosureAndGC()
    {
        // The memory can be checked with: 
        // Console.WriteLine("Memory usage: " + GC.GetTotalMemory(true));
        var mem = GC.GetTotalMemory(true);
        var f = MyMethod();
        Console.WriteLine("Memory usage: " + (GC.GetTotalMemory(true) - mem));
        string val = f("hello");
        if(val!="hello") Console.WriteLine("Captured!");
        // In my machine: If "s" was captured then memory increase over +30, else under -20
    }

    // From a lambda-point of view: x => { y + x } 
    // here x is called "bound variable" and y is a "free variable" or a "captured closure".
    // If you use free variables (defined outside the scope of the lambda) in your LINQ, 
    // always check that they are immutable and will their use cause something extra
    // e.g. memory consumption, or if y can be a function-call, y(), then this may cause 
    // multiple execution (potential performance hit) and y shouldn't have side-effects.
}
#endregion
