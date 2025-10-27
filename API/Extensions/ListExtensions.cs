using System.Collections.Generic;

namespace API.Extensions;

public static class ListExtensions {
    extension<T>(List<T> list) {
        public void RemoveFirst() {
            list.RemoveAt(0);
        }

        public void RemoveLast() {
            list.RemoveAt(list.Count - 1);
        }

        public void SwapRemove(int index) {
            list[index] = list[^1];
            list.RemoveLast();
        }

        public bool SwapRemove(T item) {
            int index = list.IndexOf(item);
            if (index < 0) return false;
            list.SwapRemove(index);
            return true;
        }
    }
}