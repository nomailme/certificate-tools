using System;
using System.Collections;
using System.Linq;

namespace certificate_tools
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this CollectionBase collection, Action<T> action)
        {
            foreach (T item in collection.OfType<T>())
            {
                action(item);
            }
        }
    }
}
