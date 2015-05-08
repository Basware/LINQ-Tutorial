using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Exercise4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Exercise task: Do not modify tests. Try to get these passed.
/// Actual code is done to the other project: LINQ-Exercises
/// </summary>
[TestClass]
public class Task_Exercise4_Generics_Fixture
{
    [TestMethod]
    public void E4_Generics_List_Execution_Test()
    {
        var arg1 = Enumerable.Repeat(3, 2).ToList();
        var arg2 = Enumerable.Empty<int>().ToList();

        var target1 = new GenericsTask<int>();

        var expected1 = new Collection<List<int>> { new List<int> { 3, 3 } };

        var result1 = target1.SecondTask<List<int>, Collection<List<int>>>(arg1, arg2);

        Assert.AreEqual(1, result1.Count);
        CollectionAssert.AreEqual(expected1.First(), result1.First());



        var arg3 = new [] {"basware", "is"};
        var arg4 = new [] { "the", "best", "company" };
        var expected2 = new List<IEnumerable<string>> { new[] { "basware", "is", "the", "best", "company" } };

        var target2 = new GenericsTask<string>();
        var result2 = target2.SecondTask<IEnumerable<string>, List<IEnumerable<string>>>(arg3, arg4);

        Assert.AreEqual(1, result2.Count);
        CollectionAssert.AreEqual(expected2.First().ToList(), result2.First().ToList());

    }
}
