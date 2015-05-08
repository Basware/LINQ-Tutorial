using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Exercise1
{
    public static class MyFiltering
    {
        // You may check the corresponding unit test asserts what is the goal.
        public static IEnumerable<ISerializable> ReturnOnlyAs(List<StringBuilder> myList)
        {
            // Todo: Maintain the functionality
            // but remove this ugly side effect of modifying myList:
            for (int i = 0; i < myList.Count; i++)
                if (myList[i].ToString() != "Item type A")
                    myList.RemoveAt(i);

            return myList;
        }
   }
}
