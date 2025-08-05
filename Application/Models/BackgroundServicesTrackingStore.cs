namespace Application.Models;

public static class BackgroundServicesTrackingStore<T>
{
    public static Dictionary<BackgroundServiceTracking, T> BackgroundServices = new Dictionary<BackgroundServiceTracking, T>();
}


//public class ObservableDictionary<TKey, TValue> :
//    IDictionary<TKey, TValue>
//    , INotifyPropertyChanged
//    , INotifyCollectionChanged
//    where TKey : notnull
//    //where TValue : 
//{
//    private readonly Dictionary<TKey, TValue> dictionary = new();

//    public event PropertyChangedEventHandler? PropertyChanged;
//    public event NotifyCollectionChangedEventHandler? CollectionChanged;

//    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
//    {
//        PropertyChanged?.Invoke(this, e);
//    }

//    private void OnPropertyChanged(TKey key)
//    {
//        OnPropertyChanged(new PropertyChangedEventArgs(key!.ToString()));
//    }

//    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
//    {
//        CollectionChanged?.Invoke(this, e);
//    }

//    private void OnCollectionChanged(NotifyCollectionChangedAction action, TKey key, TValue value)
//    {
//        OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, new KeyValuePair<TKey, TValue>(key, value)));
//    }

//    public TValue this[TKey key]
//    {
//        get => dictionary[key];
//        set
//        {
//            if (dictionary.ContainsKey(key))
//            {
//                dictionary[key] = value;
//                OnCollectionChanged(NotifyCollectionChangedAction.Add, key, value);
//                OnPropertyChanged(key);
//            }
//            else
//            {
//                Add(key, value);
//            }
//        }
//    }

//    public ICollection<TKey> Keys => dictionary.Keys;

//    public ICollection<TValue> Values => dictionary.Values;

//    public int Count => dictionary.Count;

//    public bool IsReadOnly => false;

//    public void Add(TKey key, TValue value)
//    {
//        dictionary.Add(key, value);
//        OnCollectionChanged(NotifyCollectionChangedAction.Add, key, value);
//        OnPropertyChanged(key);
//    }

//    public void Add(KeyValuePair<TKey, TValue> item)
//    {
//        Add(item.Key, item.Value);
//    }

//    public void Clear()
//    {
//        dictionary.Clear();
//        OnCollectionChanged(NotifyCollectionChangedAction.Reset, default!, default!);
//        OnPropertyChanged(null!);
//    }

//    public bool Contains(KeyValuePair<TKey, TValue> item)
//    {
//        return dictionary.Contains(item);
//    }

//    public bool ContainsKey(TKey key)
//    {
//        return dictionary.ContainsKey(key);
//    }

//    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
//    {
//        ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
//    }

//    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
//    {
//        return dictionary.GetEnumerator();
//    }

//    public bool Remove(TKey key)
//    {
//        if (dictionary.TryGetValue(key, out var value))
//        {
//            var removed = dictionary.Remove(key);
//            if (removed)
//            {
//                OnCollectionChanged(NotifyCollectionChangedAction.Remove, key, value);
//                OnPropertyChanged(key);
//            }
//            return removed;
//        }
//        return false;
//    }

//    public bool Remove(KeyValuePair<TKey, TValue> item)
//    {
//        return Remove(item.Key);
//    }

//    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
//    {
//        return dictionary.TryGetValue(key, out value);
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return GetEnumerator();
//    }
//}
