﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ibra.Enumerables
{
    public static class Extensions
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> sequence) =>
            sequence.SelectMany(sublist => sublist);

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T?> sequence) where T : struct
        {
            foreach (var nullable in sequence)
            {
                if (nullable.HasValue) yield return nullable.Value;
            }
        }

        public static IEnumerable<T> SelectWhere<U, T>(this IEnumerable<U> sequence, Func<U, T?> func) where T : struct
            => sequence.Select(func).Flatten();

        public static IEnumerable<V> TrySelectFromDictionary<K, V>(this IEnumerable<K> sequence, IReadOnlyDictionary<K, V> dictionary)
        {
            foreach (K key in sequence)
            {
                if (dictionary.TryGetValue(key, out V value)) yield return value;
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this T? maybe) where T : struct
            => maybe.HasValue ? One(maybe.Value) : Enumerable.Empty<T>();

        public static IEnumerable<T> One<T>(T item) => new SingleEnumerable<T>(item);

        public static IEnumerable<TTo> WhereCast<TFrom, TTo>(this IEnumerable<TFrom> sequence) where TTo : class, TFrom
        {
            IEnumerator<TFrom> e = sequence.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current is TTo to) yield return to;
            }
        }

        public static IEnumerable<ValueTuple<T1, T2>> Combine<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
            => first.Zip(second, ValueTuple.Create);

        public static IEnumerable<ValueTuple<int, T>> ZipWithIndex<T>(this IEnumerable<T> sequence)
            => sequence.Select((elt, idx) => ValueTuple.Create(idx, elt));
    }
}

