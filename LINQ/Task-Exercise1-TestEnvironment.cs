using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exercise1;

/// <summary>
/// Exercise task: Do not modify this class (tests). Try to get these passed.
/// Actual code is done to the other project: LINQ-Exercises
/// 
/// Usually the answer can be expressed as one line of code per exercise.
/// The total characters needed to solve all these tasks is under 350 (with spaces).
/// No problem if you can't do that. However, if you find your self doing over 10 lines 
/// of code per exercise, you are probably doing it wrong. 
/// </summary>
[TestClass]
public class Task_Exercise1_TestEnvironment
{

    [TestMethod]
    public void E1_FirstUnitTest_Dont_Modify_My_Collections_Test()
    {
        // This first exercise will confirm that your environment is working...
        
        var builders = new List<StringBuilder>
            {
                new StringBuilder("Item type A"),
                new StringBuilder("Item type B"),
                new StringBuilder("Item type A")
            };

        // We use Covariance here.
        // Co-/Contravariance is out of topic, see links for more info
        IEnumerable<ISerializable> ser = builders;

        var myitem2 = SecondItemFromList(ser); //Should be B

        var oks = MyFiltering.ReturnOnlyAs(builders);

        Assert.AreEqual(2, oks.Count());
        Assert.AreEqual("Item type A", oks.First().ToString());
        Assert.AreEqual("Item type A", oks.Skip(1).First().ToString());

        Assert.AreEqual("Item type B", myitem2.First());
    }

    #region Some other user of the collection
    /// <summary>
    /// You don't have to modify this.
    /// Usually it would be better to not code depending on list indexes 
    /// (because then list couldn't be parallel processed), 
    /// but here is shown that it is possible.
    /// </summary>
    /// <param name="builders">list of string builders</param>
    /// <returns>Item type B</returns>
    private IEnumerable<string> SecondItemFromList(IEnumerable<ISerializable> builders)
    {
        // Some maybe useful tools: "LinqPad", "Test Driven .NET", ReSharper

        // Numbers from 1 to 10000:
        var range = Enumerable.Range(1, 10000);

        // Create indexes for items:
        var indexedBuildersAll = builders.Zip(range, (item, idx) => new {item, idx});

        // Select the second index from the list:
        var myItem = from ib in indexedBuildersAll
                     where ib.idx == 2
                     select ib.item.ToString();

        return myItem;
    }
    #endregion
}
