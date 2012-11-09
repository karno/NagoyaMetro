using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Hailstone.Linq
{
    public static class EnumerableEx
    {
        public static IEnumerable<T> Unfold<T>(this Func<T, T> func, T initial)
        {
            var value = initial;
            yield return value;
            while (true)
            {
                value = func(value);
                yield return value;
            }
        }

        public static IOrderedEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
        {
            return Shuffle(source, new RNGCryptoServiceProvider());
        }

        public static IOrderedEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source, 
            RandomNumberGenerator rng)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var bytes = new byte[4];

            return source.OrderBy(delegate(TSource e)
            {
                rng.GetBytes(bytes);

                return BitConverter.ToInt32(bytes, 0);
            });
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] item)
        {
            return source.Concat(item);
        }

        public static IEnumerable<Tuple<T, U>> Let<T, U>(this IEnumerable<T> source, Func<IEnumerable<T>, U> valueGenerator)
        {
            var spawn = valueGenerator(source);
            return source.Select(s => new Tuple<T, U>(s, spawn));
        }

        public static IEnumerable<T> Singlize<T, U>(this IEnumerable<T> source, Func<IEnumerable<T>, U> valueGenerator, Action<T, U> commit)
        {
            return source.Let(i => valueGenerator(i))
                .Do(a => commit(a.Item1, a.Item2))
                .Select(i => i.Item1);
        }

        public static IEnumerable<T> WhereFork<T>(this IEnumerable<T> source, Func<T, bool> chooser, Action<T> passed)
        {
            foreach (var v in source)
            {
                if (chooser(v))
                    passed(v);
                yield return v;
            }
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T original, T replace)
        {
            foreach (var t in source)
            {
                if (t.Equals(original))
                    yield return replace;
                else
                    yield return t;
            }
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, Func<T, bool> replPredicate, T replace)
        {
            foreach (var t in source)
            {
                if (replPredicate(t))
                    yield return replace;
                else
                    yield return t;
            }
        }

        public static IEnumerable<IEnumerable<T>> Block<T>(this IEnumerable<T> collection, int count)
        {
            List<T> buffer = new List<T>();
            int i = 0;
            foreach (var item in collection)
            {
                buffer.Add(item);
                i++;
                if (i >= count)
                {
                    yield return buffer;
                    buffer.Clear();
                    i = 0;
                }
            }
            if (buffer.Count() > 0)
                yield return buffer;
        }

        public static IEnumerable<T> Guard<T>(this IEnumerable<T> collection)
        {
            return collection ?? new T[0];
        }

        public static IEnumerable<T> Guard<T>(this IEnumerable<T> collection, Func<bool> guard)
        {
            if (guard())
                return collection;
            else
                return new T[0];
        }

        public static IEnumerable<T> Guard<T>(this IEnumerable<T> collection, Func<IEnumerable<T>, bool> guard)
        {
            if (guard(collection))
                return collection;
            else
                return new T[0];
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> execute)
        {
            foreach (var i in source)
                execute(i);
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var i in collection)
            {
                action(i);
                yield return i;
            }
        }

        public static IEnumerable<TResult> SafeSelect<TSource, TResult>(this IEnumerable<TSource> collections,
            Func<TSource, TResult> functor)
        {
            return collections.SafeSelect<TSource, Exception, TResult>(functor, (_, __) => default(TResult))
                .Where(t => t.Equals(default(TResult)));
        }

        public static IEnumerable<TResult> SafeSelect<TSource, TException, TResult>(this IEnumerable<TSource> collection,
            Func<TSource, TResult> functor, Func<TSource, TException, TResult> catcher) where TException : Exception
        {
            TResult result;
            foreach (var item in collection)
            {
                try
                {
                    result = functor(item);
                }
                catch (TException tex)
                {
                    result = catcher(item, tex);
                }
                yield return result;
            }
        }

        public static string JoinString(this IEnumerable<string> source, string separator)
        {
            return String.Join(separator, source);
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> functor)
        {
            var e0 = first.GetEnumerator();
            var e1 = second.GetEnumerator();
            while (e0.MoveNext() && e1.MoveNext())
            {
                yield return functor(e0.Current, e1.Current);
            }
        }

        public static TResult Using<T, TResult>(this T disposable, Func<T, TResult> func) where T : IDisposable
        {
            using (disposable) return func(disposable);
        }
    }
}
