using System.Collections.Generic;
using System.Linq;

namespace Exercise4
{
    public class GenericsTask<T1>
    {
        /// <summary>
        /// This will input two lists of any type
        /// </summary>
        /// <typeparam name="T2">any list type</typeparam>
        /// <typeparam name="T3">any list type</typeparam>
        /// <param name="inputList1">first list</param>
        /// <param name="inputList2">second list</param>
        /// <returns>list of lists: first item of that is those input lists combined! </returns>
        public T3 SecondTask<T2, T3>(T2 inputList1, T2 inputList2)
            where T2 : class, IEnumerable<T1>
            where T3 : ICollection<T2>, new()
        {
            // Todo: You should add/modify few lines of code here to get the test to pass:
            return new T3();
        }
    }
}

#region Some tips as this task is also a bit hard
// - If you have read the material and 
// - And look the signature of the return method
// - This is exercise of Generics. This is not example of pure type driven programming.
//   Type driven programming would be better, but C# has some practical limitations.
// - Inside T3 you need some instance that will fulfil the conditions of T2 : class, IEnumerable<T1>
#endregion
