using System.Collections.Generic;

namespace Exercise2
{
    public static class MyExtensionsTasks
    {
        // Try to yield something but not everything...
        public static IEnumerable<long> MyExtensionMethodYieldTask(this IEnumerable<long> xs)
        {
            // Todo: You should add/modify few lines of code here to get the test to pass:
            foreach (var x in xs)
            {
                yield return x;
            }
        }

   }
}
