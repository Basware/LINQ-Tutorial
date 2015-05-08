using System;

namespace Exercise3
{
    public class MyFunction
    {
        // Factorial is calculated as 1 * 2 * 3 * ... * n
        // First function is the recursion, second is the current of n.
        public static Func<Func<int, int>, Func<int, int>> CalculateFactorial()
        {
            // Todo: You should add/modify few lines of code here to get the test to pass:
            throw new NotImplementedException("Todo: Calculate factorial");
            return y => y;

            #region Some tips as this is the harderst of the exercises:
            // This is exercise of Func<T> types.
            // Add breakpoint inside lambda
            // Add watch to your variables

            // Calling y will create a recursive loop.
            // To get that working, you will need:
            // 1) Call to this y function with your parameters
            // 2) Before calling y, make some kind of stop condition
            #endregion
        }
    }
}
