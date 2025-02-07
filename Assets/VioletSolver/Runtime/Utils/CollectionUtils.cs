using System.Collections.Generic;
using System.Linq;

namespace VioletSolver
{
    internal static class CollectionUtils<T>
    {
        internal static void ExpandList(int refSize, ref List<T> list)
        {
            var listSize = list.Count;
            var diff = refSize - listSize;
            if (diff <= 0) return;
            list.AddRange(new T[diff].ToList());
        }
    }
}
