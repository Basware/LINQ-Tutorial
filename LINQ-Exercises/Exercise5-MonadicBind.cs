using System;

namespace Exercise5
{
    public static class MyEurosTask
    {
        // What happens inside SelectMany: 
        // First evaluate valueSelector, then resultSelector
        public static TOut Bind<TIn, TIn2, TOut>(EUR<TIn> source, Func<TIn, EUR<TIn2>> valueSelector, Func<TIn, TIn2, TOut> resultSelector)
        {
            // Todo: You should add/modify few lines of code here to get the test pass:
            return default(TOut);
        }

    }

    /// <summary>
    /// Container of EUROs. Don't need to modify this.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EUR<T>
    {
        public EUR(T i)
        {
            Total = i;
        }

        public T Total { get; private set; }

    }
}