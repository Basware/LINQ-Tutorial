using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Exercise6;

[TestClass]
public class Task_Exercise6_Catamorphism_Fixture
    {
    [TestMethod]
    public void E6_Tree_Aggregation_Test()
    {
        // This is our tree:
        //     4 
        //  2     6 
        // 1 3   5 7 
        var tree7 = new Tree<int>(4,
                                  new Tree<int>(2, new Tree<int>(1), new Tree<int>(3)),
                                  new Tree<int>(6, new Tree<int>(5), new Tree<int>(7)));

        var heightTree = tree7.Aggregate((_, l, r) => 1 + (l > r ? l : r), 0);
        Console.WriteLine(heightTree); // 3

        var multiplyTree = tree7.Aggregate((x, l, r) => x*l*r, 1);
        Console.WriteLine(multiplyTree); // 5040

        // This is our treeA:
        //   b 
        //  c d 
        var treeA = new Tree<string>("b", new Tree<string>("c"), new Tree<string>("d"));
        var res1 = treeA.Aggregate(Catamorphism.SumTree(), string.Empty);
        Assert.AreEqual("bcd", res1); 

        // This is our treeB:
        //      a 
        //    b   e
        //   c d 
        var treeB = new Tree<string>("a", treeA, new Tree<string>("e"));
        var res2 = treeB.Aggregate(Catamorphism.SumTree(), string.Empty);
        Assert.AreEqual("abcde", res2);
    }

}

#region Tree structure

// Let's first make a simple node structure for a binary tree:
public class Node<TData, TLeft, TRight>
{
    public TLeft Left { get; private set; }
    public TRight Right { get; private set; }
    public TData Data { get; private set; }
    public Node(TData x, TLeft l, TRight r) { Data = x; Left = l; Right = r; }
}

// Then, here is a tree, as a recursive type:

public class Tree<T> : Node</* data: */ T, /* left: */ Tree<T>, /* right: */ Tree<T>>
{
    public Tree(T data, Tree<T> left, Tree<T> right) : base(data, left, right) { }
    public Tree(T data) : base(data, null, null) { }
}
#endregion

#region Fold

public static class TreeExtensions
{
    private static R Loop<A, R>(Func<A, R, R, Tree<A>, R> nodeF, Func<Tree<A>, R> leafV, Tree<A> t, Func<R, R> cont)
    {
        if (t == null) return cont(leafV(t));
        return Loop(nodeF, leafV, t.Left, lacc =>
                Loop(nodeF, leafV, t.Right, racc =>
                cont(nodeF(t.Data, lacc, racc, t))));
    }

    public static R XAggregateTree<A, R>(this Tree<A> tree, Func<A, R, R, Tree<A>, R> nodeF, Func<Tree<A>, R> leafV)
    {
        return Loop(nodeF, leafV, tree, x => x);
    }

    public static R Aggregate<A, R>(this Tree<A> tree, Func<A, R, R, R> nodeF, R leafV)
    {
        return tree.XAggregateTree((x, l, r, _) => nodeF(x, l, r), _ => leafV);
    }
}
#endregion

