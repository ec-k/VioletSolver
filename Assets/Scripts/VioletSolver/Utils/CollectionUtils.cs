using System.Collections.Generic;
using System.Linq;

namespace VioletSolver
{
    public static class CollectionUtils<T>
    {
        public static void ExpandList(int refSize, ref List<T> list)
        {
            var listSize = list.Count;
            var diff = refSize - listSize;
            if (diff <= 0) return;
            list.AddRange(new T[diff].ToList());
        }
    }
}
