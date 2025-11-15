using System.Collections.Generic;

namespace API.Extensions;

public static class ListExtensions {
    extension<T>(List<T> @this) {
        public void RemoveFirst() {
            @this.RemoveAt(0);
        }

        public void RemoveLast() {
            @this.RemoveAt(@this.Count - 1);
        }

        public void SwapRemove(int index) {
            @this[index] = @this[^1];
            @this.RemoveLast();
        }

        public bool SwapRemove(T item) {
            int index = @this.IndexOf(item);
            if (index < 0) return false;
            @this.SwapRemove(index);
            return true;
        }
    }
}