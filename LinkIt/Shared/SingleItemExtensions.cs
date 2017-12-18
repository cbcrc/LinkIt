using System.Collections.Generic;

namespace LinkIt.Shared
{
    //Adapted from http://stackoverflow.com/questions/1577822/passing-a-single-item-as-ienumerablet
    public static class SingleItemExtensions
    {
        /// <summary>
        ///     Wraps this object instance into an IEnumerable&lt;T&gt;
        ///     consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            if (item == null) yield break;

            yield return item;
        }
    }
}