using System.Collections.Generic;

namespace API.Extensions;

public static class ListExtensions
{
    extension<T>(List<T> @this)
    {
        /// <summary>
        /// Removes the first element from the list
        /// </summary>
        public void RemoveFirst()
        {
            @this.RemoveAt(0);
        }

        /// <summary>
        /// Removes the last element from the list
        /// </summary>
        public void RemoveLast()
        {
            @this.RemoveAt(@this.Count - 1);
        }

        /// <summary>
        /// Removes the <c>index</c>th element from the list, and swaps the last element into its place
        /// </summary>
        public void SwapRemove(int index)
        {
            @this[index] = @this[^1];
            @this.RemoveLast();
        }

        /// <summary>
        /// Searches the list for <c>item</c>. If found, removes it and swaps the last element into its place
        /// </summary>
        /// <returns>
        /// Whether the removal suceeded
        /// </returns>
        public bool SwapRemove(T item)
        {
            int index = @this.IndexOf(item);
            
            if (index < 0)
            {
                return false;
            }

            @this.SwapRemove(index);
            return true;
        }
    }
}