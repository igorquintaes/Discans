using System;
using System.Collections.Generic;

namespace Discans.Extensions
{
    public static class ListExtension
    {
        public static IEnumerable<List<T>> ChunkList<T>(this List<T> locations, int nSize)
        {
            for (int i = 0; i < locations.Count; i += nSize)
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
        }
    }
}
