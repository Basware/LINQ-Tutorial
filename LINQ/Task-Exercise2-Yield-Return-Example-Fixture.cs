using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exercise2;

/// <summary>
/// Exercise task: Do not modify tests. Try to get these passed.
/// Actual code is done to the other project: LINQ-Exercises
/// </summary>
[TestClass]
public class Task_Exercise2_Yield_Return_Example_Fixture
{

    [TestMethod]
    public void E2_ExtensionMethodAndYieldReturn_Exercise_Test()
    {
        var expected = Enumerable.Range(1, 3).ToList();
        var result = yielder().MyExtensionMethodYieldTask().Select(i => 12/i).Select(x => (int)x).ToList();

        CollectionAssert.AreEqual(expected, result);
    }


    IEnumerable<long> yielder()
    {
        yield return 0; //skip this
        yield return 12;
        yield return 6;
        yield return 0; //skip this
        yield return 4;
        yield return 1;
        //Thread.Sleep(int.MaxValue);
        throw new InvalidOperationException("Please, don't go this far!");
    }
            
}
