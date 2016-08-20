using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Ann.Foundation.Mvvm
{
    public static class ModelHelper
    {
        public static void MovoTo<T>(ObservableCollection<T> collection, T item, int newIndex)
        {
            var oldIndex = collection.IndexOf(item);
            Debug.Assert(oldIndex != -1);

            newIndex = Math.Min(newIndex, collection.Count);

            if (oldIndex < newIndex)
                -- newIndex;

            collection.Move(oldIndex, newIndex);
        }
    }
}